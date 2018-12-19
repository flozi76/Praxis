using System.Collections.Generic;
using Duftfinder.Domain.Dtos;

namespace Duftfinder.Web.Models
{
	/// <summary>
	///     Represents the model with the results of the essential oils  that is used for the view.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class SearchResultViewModel
	{
		/// <summary>
		///     Creates a list of alphabetical indexes of essential oils. (A-Z).
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<string> AlphabeticalIndexes
		{
			get
			{
				IList<string> firstLetterList = new List<string>();
				foreach (var essentialOilViewModel in SearchEssentialOilResults)
				{
					// Add first letter to list, if it doesn't already exist.
					var firstLetter = essentialOilViewModel.Name.Substring(0, 1).ToUpper();
					if (!firstLetterList.Contains(firstLetter)) firstLetterList.Add(firstLetter);
				}

				return firstLetterList;
			}
		}

		/// <summary>
		///     The list with the essential oil results.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<EssentialOilViewModel> SearchEssentialOilResults { get; set; }

		/// <summary>
		///     The amount of Essential Oils that where found with the search.
		/// </summary>
		/// <author>Anna Krebs</author>
		public int SearchEssentialOilResultsAmount { get; set; }

		/// <summary>
		///     The list with the searched effects.
		/// </summary>
		/// <author>Anna Krebs</author>
		public IList<SearchEffectItem> SearchEffects { get; set; }

		/// <summary>
		///     The text of the searched essential oil.
		/// </summary>
		/// <author>Anna Krebs</author>
		public string SearchEssentialOilText { get; set; }
	}
}