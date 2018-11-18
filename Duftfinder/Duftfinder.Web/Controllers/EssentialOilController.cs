using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
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
    /// The Controller for all "Ätherisches Öle" stuff in the "Adminbereich".
    /// <author>Anna Krebs</author>
    /// </summary>
    [Authorize(Roles = Constants.Admin)]
    public class EssentialOilController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEssentialOilService _essentialOilService;

        private readonly IMoleculeService _moleculeService;

        private readonly ISubstanceService _substanceService;

        private readonly IEssentialOilMoleculeService _essentialOilMoleculeService;

        private readonly IEffectService _effectService;

        private readonly ICategoryService _categoryService;

        private readonly IEssentialOilEffectService _essentialOilEffectService;

        private readonly ConversionHelper _conversionHelper = new ConversionHelper();

        public EssentialOilController(IEssentialOilService essentialOilService, IMoleculeService moleculeService, ISubstanceService substanceService, IEssentialOilMoleculeService essentialOilMoleculeService, IEffectService effectService, ICategoryService categoryService, IEssentialOilEffectService essentialOilEffectService)
        {
            _essentialOilService = essentialOilService;
            _moleculeService = moleculeService;
            _substanceService = substanceService;
            _essentialOilMoleculeService = essentialOilMoleculeService;
            _effectService = effectService;
            _categoryService = categoryService;
            _essentialOilEffectService = essentialOilEffectService;
        }

        public async Task<ActionResult> Index(string lastEditedEssentialOilId)
        {
            EssentialOilFilter filter = new EssentialOilFilter();

            // Get all essential oils from database.
            IList<EssentialOil> essentialOils = await _essentialOilService.GetAllAsync(filter);
            IList<EssentialOilViewModel> essentialOilViewModels = new List<EssentialOilViewModel>();

            // Create list of essential oils for view. 
            foreach (EssentialOil essentialOil in essentialOils)
            {
                essentialOilViewModels.Add(new EssentialOilViewModel(essentialOil));
            }

            // Create view model for Index view.
            EssentialOilViewModelIndex essentialOilViewModelIndex = new EssentialOilViewModelIndex(essentialOilViewModels);

            // Set value to where to scroll to.
            essentialOilViewModelIndex.LastEditedEssentialOilId = lastEditedEssentialOilId;

            return View(essentialOilViewModelIndex);
        }

        /// <summary>
        /// Shows view for create or edit of essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(string id)
        {
            EssentialOilViewModel model;

            // Get EssentialOilViewModel according to whether is edit or create.
            if (!string.IsNullOrEmpty(id))
            {
                // Edit
                EssentialOil essentialOil = await _essentialOilService.GetByIdAsync(id);

                if (essentialOil == null)
                {
                    Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                    throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
                }

                model = new EssentialOilViewModel(essentialOil);
            }
            else
            {
                // Create
                model = new EssentialOilViewModel();
            }

            return View(model);
        }

        /// <summary>
        /// Creates or edits an essential oil after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(EssentialOilViewModel model, HttpPostedFileBase uploadFile)
        {
            ValidationResultList validationResult = new ValidationResultList();
            EssentialOil essentialOil = new EssentialOil();

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile?.ContentLength > 0)
                    {
                        // Get file name & base 64 string for picture.
                        essentialOil.PictureFileName = Path.GetFileName(uploadFile.FileName);
                        essentialOil.PictureDataAsString = _conversionHelper.ResizeAndGenerateBase64StringForPicture(uploadFile);
                    }

                    // Map view model to entity.
                    model.MapViewModelToEntity(essentialOil);

                    // Edit or create
                    if (essentialOil.Id != null)
                    {
                        // Edit
                        // Only update if essential oil name doesn't already exist.
                        validationResult = await _essentialOilService.UpdateAsync(essentialOil);
                    }
                    else
                    {
                        // Create
                        // Only insert if essential oil name doesn't already exist.
                        validationResult = await _essentialOilService.InsertAsync(essentialOil);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"CreateOrEdit. An unexpected error occurred while inserting or editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                Log.Info("Show CreateOrEdit");
                return View(nameof(CreateOrEdit), model);
            }

            // If form is valid, navigate to AssignMolecule.
            Log.Info("Redirect to AssignMolecule");
            return RedirectToAction(nameof(AssignMolecule), new { id = essentialOil.Id });
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
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectService.GetByFilterAsync(new EssentialOilEffectFilter { EssentialOilId = id });

            string essentialOilAlreadyAssigned = string.Empty;
            if (essentialOilEffects.Count > 0)
            {
                // Essential oil is assigned.
                essentialOilAlreadyAssigned = Resources.Resources.Confirmation_Delete_EssentialOilAlreadyAssigned;
            }

            ConfirmationViewModel model = new ConfirmationViewModel {
                Id = id, Name = name,
                DialogTitle = Resources.Resources.Confirmation_Delete_Title,
                DialogText = $"{Resources.Resources.Confirmation_Delete_Text} {essentialOilAlreadyAssigned}",
                Action = Constants.EssentialOilDelete
            };

            return PartialView("~/Views/Shared/_Confirmation.cshtml", model);
        }

        /// <summary>
        /// Deletes an essential oil after delete was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                ValidationResultList validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

                // Show validation result, if error occurred.
                if (validationResult.HasErrors)
                {
                    Log.Error($"EssentialOil with id {id} could not be deleted");
                    return new JsonErrorResult($"{validationResult.Errors.Values.SingleOrDefault()}");
                }
            }
            catch (Exception e)
            {
                // Show general error message if exception occurred.
                Log.Error($"An unexpected error occurred while deleting: {e}");
                return new JsonErrorResult($"{Resources.Resources.Error_UnexpectedError}");
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Shows view in order to assign molecules to essential oil.
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
        /// Assigns molecules to an essential oil after save was clicked.
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
                    if (assignValueViewModels == null || string.IsNullOrEmpty(model.EssentialOilId))
                    {
                        Log.Error("AssignMolecule. An unexpected error occurred. Value is null.");
                        throw new ArgumentNullException(Resources.Resources.Error_UnexpectedError);
                    }

                    // Delete all assigned molecules from database in order to update.
                    validationResult = await _essentialOilMoleculeService.DeleteAssignedMoleculesAsync(model.EssentialOilId);
                    
                    // Insert assigned molecules if deletion was successful. 
                    if (!validationResult.HasErrors)
                    {
                        foreach (AssignValueViewModel assignValueViewModel in assignValueViewModels)
                        {
                            // Insert the assigned molecules in database.
                            validationResult = await AssignMoleculeToEssentialOil(model, assignValueViewModel);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"AssignMolecule. An unexpected error occurred while inserting or editing: {e}");
                    throw;
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddGeneralModelStateError(validationResult);

                AddValidationResultsToModelStateErrors(validationResult.Errors);

                // Set substances to display in list.
                IList<Substance> substances = await _substanceService.GetAllAsync(new SubstanceFilter());
                model.Substances = substances;

                Log.Info("Show AssignMolecule");
                return View(nameof(AssignMolecule), model);
            }

            Log.Info("Redirect to AssignEffect");
            return RedirectToAction(nameof(AssignEffect), new { id = model.EssentialOilId });
        }

        /// <summary>
        /// Inserts all assigned molecules for essential oil in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModel"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> AssignMoleculeToEssentialOil(AssignMoleculeViewModel model, AssignValueViewModel assignValueViewModel)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Only assign, if MoleculePercentage was assigned.
            if (assignValueViewModel.MoleculePercentage.HasValue &&  assignValueViewModel.MoleculePercentage.Value > 0)
            {
                EssentialOilMolecule essentialOilMolecule = new EssentialOilMolecule();

                // Map view model to entity.
                essentialOilMolecule.EssentialOilId = model.EssentialOilId;
                essentialOilMolecule.MoleculeId = assignValueViewModel.AssignedValueId;
                essentialOilMolecule.MoleculePercentage = assignValueViewModel.MoleculePercentage.Value;

                validationResult = await _essentialOilMoleculeService.InsertAsync(essentialOilMolecule);
                Log.Info($"Assign molecule with id {assignValueViewModel.AssignedValueId} to essential oil with id {model.EssentialOilId}");
            }

            return validationResult;
        }

        /// <summary>
        /// Inserts all assigned effects for essential oil in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModel"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> AssignEffectToEssentialOil(AssignEssentialOilEffectViewModel model, AssignValueViewModel assignValueViewModel)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Only assign, if EffectDegree was assigned.
            if (assignValueViewModel.EffectDegree > 0)
            {
                EssentialOilEffect essentialOilEffect = new EssentialOilEffect();

                // Map view model to entity.
                essentialOilEffect.EssentialOilId = model.EssentialOilId;
                essentialOilEffect.EffectId = assignValueViewModel.AssignedValueId;
                essentialOilEffect.EffectDegree = assignValueViewModel.EffectDegree;

                validationResult = await _essentialOilEffectService.InsertAsync(essentialOilEffect);
                Log.Info($"Assign effect with id {assignValueViewModel.AssignedValueId} to essential oil with id {model.EssentialOilId}");
            }

            return validationResult;
        }

        /// <summary>
        /// Gets the AssignMoleculeViewModel.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<AssignMoleculeViewModel> GetAssignMoleculeViewModel(string id)
        {
            EssentialOil essentialOil = await _essentialOilService.GetByIdAsync(id);

            if (essentialOil == null)
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            // Get values from database.
            IList<Molecule> molecules = await _moleculeService.GetAllAsync(new MoleculeFilter());
            IList<Substance> substances = await _substanceService.GetAllAsync(new SubstanceFilter());
            IList<EssentialOilMolecule> essentialOilMolecules = await _essentialOilMoleculeService.GetAllAsync(new EssentialOilMoleculeFilter {EssentialOilId = id});

            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Create list of molecules for view. 
            foreach (Molecule molecule in molecules)
            {
                AssignValueViewModel assignValueViewModel = new AssignValueViewModel(molecule, null, null);

                foreach (EssentialOilMolecule essentialOilMolecule in essentialOilMolecules)
                {
                    // Map values from database to model.
                    if (assignValueViewModel.AssignedValueId == essentialOilMolecule.MoleculeId)
                    {
                        assignValueViewModel.MoleculePercentage = essentialOilMolecule.MoleculePercentage;
                    }
                }

                assignValueViewModels.Add(assignValueViewModel);
            }

            AssignMoleculeViewModel model = new AssignMoleculeViewModel(essentialOil, null, assignValueViewModels, substances);

            Log.Info($"Get AssignMoleculeViewModel for essential oil with id {id}");
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
            EssentialOil essentialOil = await _essentialOilService.GetByIdAsync(id);

            if (essentialOil == null)
            {
                Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
            }

            // Get values from database.
            IList<Effect> effects = await _effectService.GetAllAsync(new EffectFilter());
            IList<Category> categories = await _categoryService.GetAllAsync(new CategoryFilter());
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectService.GetAllAsync(new EssentialOilEffectFilter { EssentialOilId = id });

            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Create list of effects for view. 
            foreach (Effect effect in effects)
            {
                AssignValueViewModel assignValueViewModel = new AssignValueViewModel(null, effect, null);

                foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
                {
                    // Map values from database to model.
                    if (assignValueViewModel.AssignedValueId == essentialOilEffect.EffectId)
                    {
                        assignValueViewModel.EffectDegree = essentialOilEffect.EffectDegree;
                    }
                }

                assignValueViewModels.Add(assignValueViewModel);
            }

            AssignEssentialOilEffectViewModel model = new AssignEssentialOilEffectViewModel(essentialOil, null, assignValueViewModels, categories);

            Log.Info($"Get AssignEssentialOilEffectViewModel for essential oil with id {id}");
            return model;
        }

        /// <summary>
        /// Shows view in order to assign effect to essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AssignEffect(string id)
        {
            AssignEssentialOilEffectViewModel model;

            if (!string.IsNullOrEmpty(id))
            {
                model = await GetEssentialOilEffectViewModel(id);
            }
            else
            {
                // Navigate to Index, if id is null for some reason.
                Log.Info("AssignEffect. Redirect to Index. Id is null.");
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Assigns effects to an essential oil after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="assignValueViewModels"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AssignEffect(AssignEssentialOilEffectViewModel model, IList<AssignValueViewModel> assignValueViewModels)
        {
            ValidationResultList validationResult = new ValidationResultList();

            model.AssignEffects = assignValueViewModels;

            if (ModelState.IsValid)
            {
                try
                {
                    if (assignValueViewModels == null || string.IsNullOrEmpty(model.EssentialOilId))
                    {
                        Log.Error("AssignEffect. An unexpected error occurred. Value is null.");
                        throw new ArgumentNullException($"An unexpected error occurred. Value is null.");
                    }

                    // Delete all assigned effects from database in order to update.
                    validationResult = await _essentialOilEffectService.DeleteAssignedEffectsAsync(model.EssentialOilId);

                    // Insert assigned effects if deletion was successful. 
                    if (!validationResult.HasErrors)
                    {
                        foreach (AssignValueViewModel assignValueViewModel in assignValueViewModels)
                        {
                            // Insert the assigned effects in database.
                            validationResult = await AssignEffectToEssentialOil(model, assignValueViewModel);
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

                // Set categories to display in list.
                IList<Category> categories = await _categoryService.GetAllAsync(new CategoryFilter());
                model.Categories = categories;

                Log.Info("Show AssignEffect");
                return View(nameof(AssignEffect), model);
            }

            Log.Info("Redirect to Index");
            return RedirectToAction(nameof(Index), new { lastEditedEssentialOilId = model.EssentialOilId });
        }

        /// <summary>
        /// Converts the uploaded picture into a displayable format.
        /// Is called from javascript.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayUploadedPicture(HttpPostedFileWrapper uploadFile)
        {
            string fileName = null;
            string imageDisplayString = null;

            if (uploadFile?.ContentLength > 0)
            {
                try
                {
                    string pictureDataAsString = _conversionHelper.ResizeAndGenerateBase64StringForPicture(uploadFile);
                    imageDisplayString = $"data:image/gif;base64,{pictureDataAsString}";
                    fileName = uploadFile.FileName;
                }
                catch (ArgumentException e)
                {
                    Log.Error($"Upload of file {fileName} failed. No valid file.", e);
                    return new JsonErrorResult($"{Resources.Resources.Error_FileCannotBeUploaded} {Resources.Resources.Error_NoValidPictureFormat}");
                }
                catch (Exception e)
                {
                    Log.Error($"Upload of file {fileName} failed.", e);
                    return new JsonErrorResult(Resources.Resources.Error_FileCannotBeUploaded);
                }
            }

            // Returns JsonNetResult to js and displays picture in html.
            return new JsonNetResult { Data = new { FileName = fileName, ImageDisplayString = imageDisplayString } };
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

        /// <summary>
        /// Get ModelState errors and add them to the ValidationResult as an error in order to display the error at the top of the page.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="validationResult"></param>
        private void AddGeneralModelStateError(ValidationResultList validationResult)
        {
            List<ModelState> modelStateErrors = ModelState.Values.Where(m => m.Errors.Count != 0).ToList();
            if (!validationResult.HasErrors && modelStateErrors.Any())
            {
                validationResult.Errors.Add(string.Empty, Resources.Resources.Error_NotValidMoleculePercentage);
            }
        }
    }
}