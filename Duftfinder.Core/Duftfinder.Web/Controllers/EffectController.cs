using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Helpers;
using Duftfinder.Web.Models;
using log4net;

namespace Duftfinder.Web.Controllers
{
    /// <summary>
    /// The Controller for all "Wirkungen" stuff in the "Adminbereich".
    /// <author>Anna Krebs</author>
    /// </summary>
    [Authorize(Roles = Constants.Admin)]
    public class EffectController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEffectService _effectService;

        private readonly ICategoryService _categoryService;

        private readonly IMoleculeService _moleculeService;

        private readonly ISubstanceService _substanceService;

        private readonly IEffectMoleculeService _effectMoleculeService;

        private readonly IEssentialOilEffectService _essentialOilEffectService;

        private readonly IEssentialOilService _essentialOilService;

        public EffectController(IEffectService effectService, ICategoryService categoryService, IMoleculeService moleculeService, ISubstanceService substanceService, IEffectMoleculeService effectMoleculeService, IEssentialOilEffectService essentialOilEffectService, IEssentialOilService essentialOilService)
        {
            _effectService = effectService;
            _categoryService = categoryService;
            _moleculeService = moleculeService;
            _substanceService = substanceService;
            _essentialOilService = essentialOilService;
            _effectMoleculeService = effectMoleculeService;
            _essentialOilEffectService = essentialOilEffectService;
        }

        public async Task<ActionResult> Index(string lastEditedEffectId)
        {
            EffectFilter filter = new EffectFilter();

            // Get values from database.
            IList<Effect> effects = await _effectService.GetAllAsync(filter);
            IList<Category> categories = await _categoryService.GetAllAsync(new CategoryFilter());

            IList<EffectViewModel> effectViewModels = new List<EffectViewModel>();
            
            // Create list of effects for view. 
            foreach (Effect effect in effects)
            {
                EffectViewModel model = new EffectViewModel(effect, null);
                effectViewModels.Add(model);
            }
            
            // Create view model for Index view.
            EffectViewModelIndex effectViewModelIndex = new EffectViewModelIndex(effectViewModels, categories);

            // Set value to where to scroll to.
            effectViewModelIndex.LastEditedEffectId = lastEditedEffectId;

            return View(effectViewModelIndex);
        }

        /// <summary>
        /// Shows view for create or edit of effect.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(string id)
        {
            IList<Category> categories = await _categoryService.GetAllAsync(new CategoryFilter());

            EffectViewModel model;

            // Get EffectViewModel according to whether is edit or create.
            if (!string.IsNullOrEmpty(id))
            {
                // Edit
                Effect effect = await _effectService.GetByIdAsync(id);

                if (effect == null)
                {
                    Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                    throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
                }

                model = new EffectViewModel(effect, categories);
            }
            else
            {
                // Create
                model = new EffectViewModel(null, categories);
            }

            return View(model);
        }

        /// <summary>
        /// Creates or edits an effect after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(EffectViewModel model)
        {
            ValidationResultList validationResult = new ValidationResultList();
            Effect effect = new Effect();

            if (ModelState.IsValid)
            {
                try
                {
                    // Map view model to entity.
                    model.MapViewModelToEntity(effect);

                    // Edit or create
                    if (effect.Id != null)
                    {
                        // Edit
                        // Only update if effect name doesn't already exist.
                        validationResult = await _effectService.UpdateAsync(effect);
                    }
                    else
                    {
                        // Create
                        // Only insert if effect name doesn't already exist.
                        validationResult = await _effectService.InsertAsync(effect);
                    }

                }
                catch (Exception e)
                {
                    Log.Error($"An unexpected error occurred while inserting or editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                // Set categories to display in drop down.
                IList<Category> categories = await _categoryService.GetAllAsync(new CategoryFilter());
                model.Categories = categories;

                Log.Info("Show CreateOrEdit");
                return View(nameof(CreateOrEdit), model);
            }

            // If form is valid, navigate to AssignMolecule.
            Log.Info("Redirect to AssignMolecule");
            return RedirectToAction(nameof(AssignMolecule), new { id = effect.Id });
        }

        /// <summary>
        /// Shows delete confirmation after delete was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ShowConfirmDelete(string id, string name)
        {
            // Check for assigned molecules & show confirmation message appropriately.
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectService.GetByFilterAsync(new EssentialOilEffectFilter { EffectId = id });

            string effectAlreadyAssigned = string.Empty;
            if (essentialOilEffects.Count > 0)
            {
                // Essential oil is assigned.
                effectAlreadyAssigned = Resources.Resources.Confirmation_Delete_EffectAlreadyAssigned;
            }

            ConfirmationViewModel model = new ConfirmationViewModel {
                Id = id, Name = name,
                DialogTitle = Resources.Resources.Confirmation_Delete_Title,
                DialogText = $"{Resources.Resources.Confirmation_Delete_Text} {effectAlreadyAssigned}",
                Action = Constants.EffectDelete
            };

            return PartialView("~/Views/Shared/_Confirmation.cshtml", model);
        }

        /// <summary>
        /// Deletes an effect after delete was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                ValidationResultList validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

                // Show validation result, if error occurred.
                if (validationResult.HasErrors)
                {
                    Log.Error($"Effect with id {id} could not be deleted");
                    return new JsonErrorResult($"{validationResult.Errors.Values.SingleOrDefault() }");
                }
            }
            catch (Exception e)
            {
                // Show general error message if exception occurred.
                Log.Error($"An unexpected error occurred while deleting: {e}");
                return new JsonErrorResult($"{Resources.Resources.Error_UnexpectedError }");
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Shows view in order to assign molecules to effect.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AssignMolecule(string id)
        {
            AssignMoleculeViewModel model;

            if (!string.IsNullOrEmpty(id))
            {
                model = await GetAssignMoleculeViewModel(id);
            }
            else
            {
                // Navigate to Index, if id is null for some reason.
                Log.Info("AssignMolecule. Redirect to Index. Id is null.");
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Assigns molecules to an effect after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModels"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AssignMolecule(AssignMoleculeViewModel model, IList<AssignValueViewModel> assignValueViewModels)
        {
            ValidationResultList validationResult = new ValidationResultList();

            model.AssignMolecules = assignValueViewModels;

            if (ModelState.IsValid)
            {
                try
                {
                    if (assignValueViewModels == null || string.IsNullOrEmpty(model.EffectId))
                    {
                        Log.Error("AssignMolecule. An unexpected error occurred. Value is null.");
                        throw new ArgumentNullException($"An unexpected error occurred. Value is null.");
                    }

                    // Delete all assigned molecules from database in order to update.
                    validationResult = await _effectMoleculeService.DeleteAssignedMoleculesAsync(model.EffectId);

                    // Insert assigned molecules if deletion was successful. 
                    if (!validationResult.HasErrors)
                    {
                        foreach (AssignValueViewModel assignValueViewModel in assignValueViewModels)
                        {
                            // Insert the assigned molecules in database.
                            validationResult = await AssignMoleculeToEffect(model, assignValueViewModel);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"AssignMolecule. An unexpected error occurred while inserting or editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                // Set substances to display in list.
                IList<Substance> substances = await _substanceService.GetAllAsync(new SubstanceFilter());
                model.Substances = substances;

                Log.Info("Show AssignMolecule");
                return View(nameof(AssignMolecule), model);
            }

            Log.Info("Redirect to AssignEssentialOil");
            return RedirectToAction(nameof(AssignEssentialOil), new { id = model.EffectId });
        }

        /// <summary>
        /// Gets the AssignMoleculeViewModel.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<AssignMoleculeViewModel> GetAssignMoleculeViewModel(string id)
        {
            Effect effect = await _effectService.GetByIdAsync(id);

            if (effect == null)
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            // Get values from database.
            IList<Molecule> molecules = await _moleculeService.GetAllAsync(new MoleculeFilter());
            IList<Substance> substances = await _substanceService.GetAllAsync(new SubstanceFilter());
            IList<EffectMolecule> effectMolecules = await _effectMoleculeService.GetAllAsync(new EffectMoleculeFilter { EffectId = id });

            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Create list of molecules for view. 
            foreach (Molecule molecule in molecules)
            {
                AssignValueViewModel assignValueViewModel = new AssignValueViewModel(molecule, null, null);

                foreach (EffectMolecule effectMolecule in effectMolecules)
                {
                    // Map values from database to model.
                    if (assignValueViewModel.AssignedValueId == effectMolecule.MoleculeId)
                    {
                        assignValueViewModel.EffectDegree = effectMolecule.EffectDegree;
                    }
                }

                assignValueViewModels.Add(assignValueViewModel);
            }
            
            AssignMoleculeViewModel model = new AssignMoleculeViewModel(null, effect, assignValueViewModels, substances);

            Log.Info($"Get AssignMoleculeViewModel for effect with id {id}");
            return model;
        }

        /// <summary>
        /// Gets the AssignEssentialOilEffectViewModel.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<AssignEssentialOilEffectViewModel> GetEssentialOilEffectViewModel(string id)
        {
            Effect effect = await _effectService.GetByIdAsync(id);

            if (effect == null)
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            // Get values from database.
            IList<EssentialOil> essentialOils = await _essentialOilService.GetAllAsync(new EssentialOilFilter());
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectService.GetAllAsync(new EssentialOilEffectFilter { EffectId = id });

            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Create list of essential oils for view. 
            foreach (EssentialOil essentialOil in essentialOils)
            {
                AssignValueViewModel assignValueViewModel = new AssignValueViewModel(null, null, essentialOil);

                foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
                {
                    // Map values from database to model.
                    if (assignValueViewModel.AssignedValueId == essentialOilEffect.EssentialOilId)
                    {
                        assignValueViewModel.EffectDegree = essentialOilEffect.EffectDegree;
                    }
                }

                assignValueViewModels.Add(assignValueViewModel);
            }

            AssignEssentialOilEffectViewModel model = new AssignEssentialOilEffectViewModel(null, effect, assignValueViewModels, null);

            Log.Info($"Get AssignEssentialOilEffectViewModel for effect with id {id}");
            return model;
        }

        /// <summary>
        /// Shows view in order to assign effect to essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AssignEssentialOil(string id)
        {
            AssignEssentialOilEffectViewModel model;

            if (!string.IsNullOrEmpty(id))
            {
                model = await GetEssentialOilEffectViewModel(id);
            }
            else
            {
                // Navigate to Index, if id is null for some reason.
                Log.Info("AssignEssentialOil. Redirect to Index. Id is null.");
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Assigns essential oils to an effect after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModels"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AssignEssentialOil(AssignEssentialOilEffectViewModel model, IList<AssignValueViewModel> assignValueViewModels)
        {
            ValidationResultList validationResult = new ValidationResultList();

            model.AssignEffects = assignValueViewModels;

            if (ModelState.IsValid)
            {
                try
                {
                    if (assignValueViewModels == null || string.IsNullOrEmpty(model.EffectId))
                    {
                        Log.Error("AssignEffect. An unexpected error occurred. Value is null.");
                        throw new ArgumentNullException($"An unexpected error occurred. Value is null.");
                    }

                    // Delete all assigned essential oils from database in order to update.
                    validationResult = await _essentialOilEffectService.DeleteAssignedEssentialOilsAsync(model.EffectId);

                    // Insert assigned effects if deletion was successful. 
                    if (!validationResult.HasErrors)
                    {
                        foreach (AssignValueViewModel assignValueViewModel in assignValueViewModels)
                        {
                            // Insert the assigned effects in database.
                            validationResult = await AssignEssentialOilToEffect(model, assignValueViewModel);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"AssignEffect. An unexpected error occurred while inserting or editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                Log.Info("Show AssignEssentialOil");
                return View(nameof(AssignEssentialOil), model);
            }

            Log.Info("Redirect to Index");
            return RedirectToAction(nameof(Index), new { lastEditedEffectId = model.EffectId });
        }

        /// <summary>
        /// Inserts all assigned molecules for effect in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModel"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> AssignMoleculeToEffect(AssignMoleculeViewModel model, AssignValueViewModel assignValueViewModel)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Only assign, if EffectDegree was assigned.
            if (assignValueViewModel.EffectDegree > 0)
            {
                EffectMolecule effectMolecule = new EffectMolecule();

                // Map view model to entity.
                effectMolecule.EffectId = model.EffectId;
                effectMolecule.MoleculeId = assignValueViewModel.AssignedValueId;
                effectMolecule.EffectDegree = assignValueViewModel.EffectDegree;

                validationResult = await _effectMoleculeService.InsertAsync(effectMolecule);
                Log.Info($"Assign molecule with id {assignValueViewModel.AssignedValueId} to effect with id {model.EffectId}");
            }

            return validationResult;
        }

        /// <summary>
        /// Inserts all assigned essential oils for effect in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModel"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> AssignEssentialOilToEffect(AssignEssentialOilEffectViewModel model, AssignValueViewModel assignValueViewModel)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Only assign, if EffectDegree was assigned.
            if (assignValueViewModel.EffectDegree > 0)
            {
                EssentialOilEffect essentialOilEffect = new EssentialOilEffect();

                // Map view model to entity.
                essentialOilEffect.EffectId = model.EffectId;
                essentialOilEffect.EssentialOilId = assignValueViewModel.AssignedValueId;
                essentialOilEffect.EffectDegree = assignValueViewModel.EffectDegree;

                validationResult = await _essentialOilEffectService.InsertAsync(essentialOilEffect);
                Log.Info($"Assign essential oil with id {assignValueViewModel.AssignedValueId} to effect with id {model.EffectId}");
            }

            return validationResult;
        }

        /// <summary>
        /// Add validation results errors to ModelState in order to show in validation summary on view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="errors"></param>
        private void AddValidationResultsToModelStateErrors(IDictionary<string, string> errors)
        {
            foreach (KeyValuePair<string, string> error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Value);
            }
        }
    }
}