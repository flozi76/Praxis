namespace Duftfinder.Domain.Dtos
{
    /// <summary>
    /// Represents the search criteria for the effect search.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class SearchEffectItem
    {
        /// <summary>
        /// The searched effect text.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string SearchEffectText { get; set; }

        /// <summary>
        /// The value of the slider that indicates the "Beschwerdeausmass".
        /// </summary>
        /// <author>Anna Krebs</author>
        public int DiscomfortValue { get; set; }
    }
}