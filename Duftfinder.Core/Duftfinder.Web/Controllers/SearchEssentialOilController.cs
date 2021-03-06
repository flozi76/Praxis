﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Duftfinder.Web.Controllers
{
	/// <summary>
	///     The Controller for the "Ätherisches Öl suchen" stuff.
	///     "Ätherisches Öl suchen" is accessible for everyone.
	///     <author>Anna Krebs</author>
	/// </summary>
	public class SearchEssentialOilController : Controller
	{
		//private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IEssentialOilService _essentialOilService;
		private readonly ILogger<SearchEssentialOilController> _log;

		public SearchEssentialOilController(IEssentialOilService essentialOilService,
			ILogger<SearchEssentialOilController> log)
		{
			_essentialOilService = essentialOilService;
			_log = log;
		}

		public ActionResult Index(string searchEssentialOilText)
		{
			_log.LogInformation("Loading Index ...");
			var searchEssentialOilViewModel = new SearchEssentialOilViewModel();
			return View(searchEssentialOilViewModel);
		}

		/// <summary>
		///     Shows partial view for search essential oil after "Zurück was clicked on essential oil details.
		///     Button is triggered in SearchEssentialOil.js -> showEssentialOilSearch
		///     PartialView html is also set there.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEssentialOilText"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Search(string searchEssentialOilText)
		{
			var searchEssentialOilViewModel = new SearchEssentialOilViewModel();
			searchEssentialOilViewModel.SearchEssentialOilText = searchEssentialOilText;

			_log.LogInformation($"Execute search for argument {searchEssentialOilText}");
			// Return PartialView as html.
			return PartialView("_Search", searchEssentialOilViewModel);
		}

		/// <summary>
		///     Shows view for details of the searched essential oil.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="essentialOilId"></param>
		/// <param name="searchEssentialOilText"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> EssentialOilDetails(string essentialOilId, string searchEssentialOilText)
		{
			if (string.IsNullOrEmpty(essentialOilId))
			{
				_log.LogError("An unexpected error occurred while getting id. No id was set.");
				throw new ArgumentNullException(
					$"{Resources.Resources.Error_UnexpectedError} {Resources.Resources.Error_TryAgainLater}");
			}

			// Get EssentialOilViewModel.
			var essentialOil = await _essentialOilService.GetByIdAsync(essentialOilId);

			if (essentialOil == null)
			{
				_log.LogError(
					$"An unexpected error occurred while getting id. No entity with id {essentialOilId} could be found.");
				throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound,
					essentialOilId));
			}

			// Get the assigned values for the essential oil.
			essentialOil.Effects = await _essentialOilService.GetAssignedEffectsForEssentialOilAsync(essentialOil.Id);
			essentialOil.Molecules =
				await _essentialOilService.GetAssignedMoleculesForEssentialOilAsync(essentialOil.Id);

			var model = new EssentialOilViewModel(essentialOil);
			model.SearchEssentialOilText = searchEssentialOilText;


			_log.LogInformation("Show EssentialOilDetails");
			// Return PartialView as html.
			return PartialView("_EssentialOilDetails", model);
		}

		/// <summary>
		///     Gets all essential oil names as an array.
		///     Is used for to initialize data for the auto complete search.
		///     Is called from SearchEssentialOil.js
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetEssentialOilNames()
		{
			//return new JsonResult("{name : test}");

			try
			{
				var filter = new EssentialOilFilter();

				// Get all essential oils from database.
				var essentialOils = await _essentialOilService.GetAllAsync(filter);

				// Create array of essential oil names.
				var essentialOilNames = essentialOils.Select(e => e.Name).ToArray();

				_log.LogInformation($"Essential oil names '{string.Join(", ", essentialOilNames)}' where loaded.");
				// TODO: check this
				//return new JsonResult { Data = essentialOilNames, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
				return new JsonResult(essentialOilNames);
			}
			catch (Exception e)
			{
				return new JsonResult(new {e.Message, e.StackTrace});
			}
		}

		/// <summary>
		///     Searches for essential oils after search was clicked & shows partial view of search results.
		///     Button is triggered in SearchEssentialOil.js -> showEssentialOilSearchResults
		///     PartialView html is also set there.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> SearchEssentialOil(SearchEssentialOilViewModel model)
		{
			//return PartialView("~/Views/SearchEssentialOil/_EssentialOilSearchResults.cshtml", new SearchResultViewModel { });


			var filter = new EssentialOilFilter {SearchText = model.SearchEssentialOilText};

			// Get all essential oils from database.
			var essentialOils = await _essentialOilService.GetByFilterAsync(filter);
			_log.LogInformation(
				$"Search result of essential oil names '{string.Join(", ", essentialOils.Select(e => e.Name))}' where found.");

			IList<EssentialOilViewModel> essentialOilViewModels = new List<EssentialOilViewModel>();

			// Create list of essential oils for view. 
			foreach (var essentialOil in essentialOils)
				essentialOilViewModels.Add(new EssentialOilViewModel(essentialOil));

			var searchResultViewModel = new SearchResultViewModel
			{
				// Order the results by essential oil name.
				SearchEssentialOilResults = essentialOilViewModels.OrderBy(m => m.Name).ToList(),
				SearchEssentialOilText = model.SearchEssentialOilText,
				SearchEssentialOilResultsAmount = essentialOilViewModels.Count
			};

			_log.LogInformation("Show EssentialOilSearchResults");
			// Return PartialView as html.
			return PartialView("~/Views/SearchEssentialOil/_EssentialOilSearchResults.cshtml", searchResultViewModel);
		}
	}
}