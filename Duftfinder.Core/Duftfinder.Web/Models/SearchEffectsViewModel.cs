using System.Collections.Generic;
using Duftfinder.Domain.Dtos;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Wirkungen suchen" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class SearchEffectsViewModel
    {

        public SearchEffectsViewModel()
        {
            // Create list with five empty search effect items in order to enter the search criteria on the view.
            SearchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
            };
        }

        /// <summary>
        /// The id of the clicked essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        public int EssentialOilId { get; set; }

        /// <summary>
        /// The list with the searched effects.
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<SearchEffectItem> SearchEffects { get; set; }
    }
}