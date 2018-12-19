using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Models;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duftfinder.Web.Controllers
{
	/// <summary>
	///     The Controller for all "Moleküle" stuff in the "Adminbereich".
	///     <author>Anna Krebs</author>
	/// </summary>
	[Authorize(Roles = Constants.Admin)]
	public class MoleculeController : Controller
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IEffectMoleculeService _effectMoleculeService;

		private readonly IEssentialOilMoleculeService _essentialOilMoleculeService;

		private readonly IMoleculeService _moleculeService;

		private readonly ISubstanceService _substanceService;

		public MoleculeController(IMoleculeService moleculeService, ISubstanceService substanceService,
			IEssentialOilMoleculeService essentialOilMoleculeService, IEffectMoleculeService effectMoleculeService)
		{
			_moleculeService = moleculeService;
			_substanceService = substanceService;
			_essentialOilMoleculeService = essentialOilMoleculeService;
			_effectMoleculeService = effectMoleculeService;
		}

		public async Task<ActionResult> Index(string lastEditedMoleculeId)
		{
			var filter = new MoleculeFilter();

			// Get values from database.
			var molecules = await _moleculeService.GetAllAsync(filter);
			var substances = await _substanceService.GetAllAsync(new SubstanceFilter());

			IList<MoleculeViewModel> moleculeViewModels = new List<MoleculeViewModel>();

			// Create list of molecules for view. 
			foreach (var molecule in molecules)
			{
				var model = new MoleculeViewModel(molecule, null);

				if (molecule.SubstanceIdString != null)
				{
					var substance = await _moleculeService.GetSubstanceForMoleculeAsync(molecule.SubstanceIdString);

					// Set name of substance for molecule.
					model.SubstanceValue = substance.Name;
				}

				moleculeViewModels.Add(model);
			}

			// Create view model for Index view.
			var moleculeViewModelIndex = new MoleculeViewModelIndex(moleculeViewModels, substances);

			// Set value to where to scroll to.
			moleculeViewModelIndex.LastEditedMoleculeId = lastEditedMoleculeId;

			return View(moleculeViewModelIndex);
		}

		/// <summary>
		///     Shows view for create or edit of molecule.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> CreateOrEdit(string id)
		{
			var substances = await _substanceService.GetAllAsync(new SubstanceFilter());

			MoleculeViewModel model;

			// Get MoleculeViewModel according to whether is edit or create.
			if (!string.IsNullOrEmpty(id))
			{
				// Edit
				var molecule = await _moleculeService.GetByIdAsync(id);

				if (molecule == null)
				{
					Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
					throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
				}

				model = new MoleculeViewModel(molecule, substances);
			}
			else
			{
				// Create
				model = new MoleculeViewModel(null, substances);
			}

			return View(model);
		}

		/// <summary>
		///     Creates or edits a molecule after save was clicked.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> CreateOrEdit(MoleculeViewModel model)
		{
			var validationResult = new ValidationResultList();

			if (ModelState.IsValid)
				try
				{
					var molecule = new Molecule();

					// Map view model to entity.
					model.MapViewModelToEntity(molecule);

					// Edit or create
					if (molecule.Id != null)
					{
						// Edit
						// Only update if molecule name doesn't already exist.
						validationResult = await _moleculeService.UpdateAsync(molecule);
					}
					else
					{
						// Create
						// Only insert if molecule name doesn't already exist.
						validationResult = await _moleculeService.InsertAsync(molecule);
						model.Id = molecule.Id;
					}
				}
				catch (Exception e)
				{
					Log.Error($"An unexpected error occurred while inserting or editing: {e}");
					throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
				}

			// Show validation result, if validation error occurred while 
			// inserting or if ModelState is invalid.
			if (validationResult.HasErrors || !ModelState.IsValid)
			{
				AddValidationResultsToModelStateErrors(validationResult.Errors);

				// Set substances to display in drop down.
				var substances = await _substanceService.GetAllAsync(new SubstanceFilter());
				model.Substances = substances;

				Log.Info("Show CreateOrEdit");
				return View(nameof(CreateOrEdit), model);
			}

			Log.Info("Redirect to Index");
			return RedirectToAction(nameof(Index), new {lastEditedMoleculeId = model.Id});
		}

		/// <summary>
		///     Shows delete confirmation after delete was clicked.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> ShowConfirmDelete(string id, string name)
		{
			// Check for assigned molecules & show confirmation message appropriately.
			var essentialOilMolecules =
				await _essentialOilMoleculeService.GetByFilterAsync(new EssentialOilMoleculeFilter {MoleculeId = id});
			var effectMolecules =
				await _effectMoleculeService.GetByFilterAsync(new EffectMoleculeFilter {MoleculeId = id});

			var moleculeAlreadyAssigned = string.Empty;
			if (essentialOilMolecules.Count > 0 || effectMolecules.Count > 0)
				moleculeAlreadyAssigned = Resources.Resources.Confirmation_Delete_MoleculeAlreadyAssigned;

			var model = new ConfirmationViewModel
			{
				Id = id, Name = name,
				DialogTitle = Resources.Resources.Confirmation_Delete_Title,
				DialogText = $"{Resources.Resources.Confirmation_Delete_Text} {moleculeAlreadyAssigned}",
				Action = Constants.MoleculeDelete
			};

			return PartialView("~/Views/Shared/_Confirmation.cshtml", model);
		}

		/// <summary>
		///     Deletes a molecule after delete was clicked.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> Delete(string id)
		{
			try
			{
				var validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

				// Show validation result, if error occurred.
				if (validationResult.HasErrors)
				{
					Log.Error($"Molecule with id {id} could not be deleted");
					return new JsonResult($"{validationResult.Errors.Values.SingleOrDefault()}");
				}
			}
			catch (Exception e)
			{
				// Show general error message if exception occurred.
				Log.Error($"An unexpected error occurred while deleting: {e}");
				return new JsonResult($"{Resources.Resources.Error_UnexpectedError}");
			}

			return new EmptyResult();
		}

		/// <summary>
		///     Add validation results errors to ModelState in order to show in validation summary on view.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="errors"></param>
		private void AddValidationResultsToModelStateErrors(IDictionary<string, string> errors)
		{
			foreach (var error in errors) ModelState.AddModelError(string.Empty, error.Value);
		}
	}
}