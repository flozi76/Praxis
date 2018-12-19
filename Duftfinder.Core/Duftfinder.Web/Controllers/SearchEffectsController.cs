using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Models;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duftfinder.Web.Controllers
{
	/// <summary>
	///     The Controller for the "Wirkungen suchen" stuff.
	///     <author>Anna Krebs</author>
	/// </summary>
	[Authorize]
	public class SearchEffectsController : Controller
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IEffectService _effectService;

		private readonly IEssentialOilService _essentialOilService;

		public SearchEffectsController(IEssentialOilService essentialOilService, IEffectService effectService)
		{
			_essentialOilService = essentialOilService;
			_effectService = effectService;
		}

		public ActionResult Index()
		{
			var searchEffectsViewModel = new SearchEffectsViewModel();
			return View(searchEffectsViewModel);
		}

		/// <summary>
		///     Shows partial view for search effect after "Zurück" was clicked on essential oil details.
		///     Button is triggered in SearchEffects.js -> showEffectsSearch
		///     PartialView html is also set there.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEffects"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Search(IList<SearchEffectItem> searchEffects)
		{
			var searchEffectsViewModel = new SearchEffectsViewModel();
			searchEffectsViewModel.SearchEffects = searchEffects;

			Log.Info("Show Search");
			// Return PartialView as html.
			return PartialView("_Search", searchEffectsViewModel);
		}

		/// <summary>
		///     Shows partial view for details of the searched essential oil after details was clicked.
		///     Button is triggered in SearchEffects.js -> showEssentialOilDetails
		///     PartialView html is also set there.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEffects"></param>
		/// <param name="essentialOilId"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> EssentialOilDetails(List<SearchEffectItem> searchEffects, string essentialOilId)
		{
			if (string.IsNullOrEmpty(essentialOilId))
			{
				Log.Error("An unexpected error occurred while getting id. No id was set.");
				throw new ArgumentNullException(
					$"{Resources.Resources.Error_UnexpectedError} {Resources.Resources.Error_TryAgainLater}");
			}

			// Get EssentialOilViewModel.
			var essentialOil = await _essentialOilService.GetByIdAsync(essentialOilId);

			if (essentialOil == null)
			{
				Log.Error(
					$"An unexpected error occurred while getting id. No entity with id {essentialOilId} could be found.");
				throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound,
					essentialOilId));
			}

			// Get the assigned values for the essential oil.
			essentialOil.Effects = await _essentialOilService.GetAssignedEffectsForEssentialOilAsync(essentialOil.Id);
			essentialOil.Molecules =
				await _essentialOilService.GetAssignedMoleculesForEssentialOilAsync(essentialOil.Id);

			var model = new EssentialOilViewModel(essentialOil);

			// Add searched effects to model, in order to restore them later, if "Zurück" is clicked.
			model.SearchEffects = searchEffects;

			Log.Info("Show EssentialOilDetails");
			// Return PartialView as html.
			return PartialView("~/Views/SearchEssentialOil/_EssentialOilDetails.cshtml", model);
		}

		/// <summary>
		///     Gets all effect names as an array.
		///     Is used for to initialize data for the auto complete search.
		///     Is called from SearchEffects.js
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetEffectNames()
		{
			var filter = new EffectFilter();

			// Get all effects from database.
			var effects = await _effectService.GetAllAsync(filter);

			// Create array of effect names.
			var effectNames = effects.Select(e => e.Name).ToArray();

			Log.Info($"Effect names '{string.Join(", ", effectNames)}' where loaded.");

			// TODO: check this
			//return new JsonResult {Data = effectNames, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
			return new JsonResult(effectNames);
		}

		/// <summary>
		///     Searches for essential oils after search was clicked & shows partial view of search results.
		///     Button is triggered in SearchEffects.js -> showEffectsSearchResults
		///     PartialView html is also set there.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEffects"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ActionResult> SearchEssentialOil(List<SearchEffectItem> searchEffects)
		{
			// Remove empty search items.
			searchEffects.RemoveAll(s => string.IsNullOrWhiteSpace(s.SearchEffectText) || s.DiscomfortValue < 1);

			// Remove duplicate search items.
			searchEffects = RemoveDuplicateSearchEffects(searchEffects);

			// Calculate the MaxEffectDegreeDiscomfortValue. Value is used to calculate the WeightedMatchValue.
			// maxEffectDegreeDiscomfortValue -> multiply all inputted slider DiscomfortValues * 4 and add these values.
			// E.g. 3 Sliders with inputted DiscomfortValue 4, 1, 2.
			// MaxEffectDegreeDiscomfortValue would be (4 * 4) + (1 * 4) + (2 * 4) = 28
			var maxEffectDegreeDiscomfortValue = 0;
			foreach (var searchEffect in searchEffects)
				maxEffectDegreeDiscomfortValue = maxEffectDegreeDiscomfortValue + searchEffect.DiscomfortValue * 4;

			// Get all essential oils, that are assigned to the searched effects.
			var searchEssentialOilItemsResults =
				await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchEffects);
			Log.Info(
				$"Search result of essential oil names '{string.Join(", ", searchEssentialOilItemsResults.Select(e => e.EssentialOil.Name))}' where found.");

			var essentialOilViewModels =
				CreateEssentialOilViewModels(searchEssentialOilItemsResults, maxEffectDegreeDiscomfortValue);

			var searchResultViewModel = new SearchResultViewModel
			{
				SearchEssentialOilResults = essentialOilViewModels,
				SearchEffects = searchEffects,
				SearchEssentialOilResultsAmount = essentialOilViewModels.Count
			};

			Log.Info("Show EffectSearchResults");
			// Return PartialView as html.
			return PartialView("~/Views/SearchEffects/_EffectsSearchResults.cshtml", searchResultViewModel);
		}

		/// <summary>
		///     Creates the essential oil view model that is shown as the result of the effect search.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEssentialOilItemsResults"></param>
		/// <param name="maxEffectDegreeDiscomfortValue"></param>
		/// <returns></returns>
		private IList<EssentialOilViewModel> CreateEssentialOilViewModels(
			IList<SearchEssentialOilItem> searchEssentialOilItemsResults, int maxEffectDegreeDiscomfortValue)
		{
			// Order the results by descending MatchAmount, then by descending EffectDegreeDiscomfortValue, then essential oil name.
			searchEssentialOilItemsResults = searchEssentialOilItemsResults
				.OrderByDescending(m => m.MatchAmount)
				.ThenByDescending(m => m.EffectDegreeDiscomfortValue)
				.ThenBy(m => m.EssentialOil.Name)
				.ToList();

			// Create the view model with the result list.
			IList<EssentialOilViewModel> essentialOilViewModels = new List<EssentialOilViewModel>();
			foreach (var resultItem in searchEssentialOilItemsResults)
			{
				// Get the specific result item and add to view model.
				var essentialOilViewModel = new EssentialOilViewModel(resultItem.EssentialOil);
				essentialOilViewModel.EffectDegreeDiscomfortValue = resultItem.EffectDegreeDiscomfortValue;
				essentialOilViewModel.MatchAmount = resultItem.MatchAmount;
				essentialOilViewModel.WeightedMatchValue =
					_essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);
				essentialOilViewModel.SearchEffectTextsInEssentialOil = resultItem.SearchEffectTextsInEssentialOil;

				essentialOilViewModels.Add(essentialOilViewModel);
			}

			return essentialOilViewModels;
		}

		/// <summary>
		///     Removes duplicates, if in effect search same search effects where entered.
		///     Takes highest DiscomfortValue, if these values differ in the duplicates.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="searchEffects"></param>
		/// <returns></returns>
		private List<SearchEffectItem> RemoveDuplicateSearchEffects(List<SearchEffectItem> searchEffects)
		{
			// Trim search effect text.
			searchEffects.ForEach(s => { s.SearchEffectText = s.SearchEffectText.Trim(); });

			// Get list with duplicated searched effects.
			var duplicateSearchedEffects =
				searchEffects.GroupBy(s => s.SearchEffectText).Where(g => g.Count() > 1).ToList();

			// Remove all duplicate searched effects except the one with the highest DiscomfortValue.
			foreach (var duplicateSearchedEffect in duplicateSearchedEffects)
			{
				var duplicateWithHighestDiscomfortValue = searchEffects
					.Where(se => se.SearchEffectText == duplicateSearchedEffect.Key)
					.OrderByDescending(s => s.DiscomfortValue).FirstOrDefault();

				searchEffects.RemoveAll(s => s.SearchEffectText == duplicateSearchedEffect.Key);
				searchEffects.Add(duplicateWithHighestDiscomfortValue);
			}

			return searchEffects;
		}
	}
}