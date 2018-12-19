using Duftfinder.Domain.Entities;

namespace Duftfinder.Domain.Dtos
{
	/// <summary>
	///     Represents the essential oil search item.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class SearchEssentialOilItem
	{
		public string EssentialOilId { get; set; }

		public EssentialOil EssentialOil { get; set; }

		/// <summary>
		///     Product of "Wirksamkeit" * "Beschwerdeausmass".
		/// </summary>
		/// <author>Anna Krebs</author>
		public int EffectDegreeDiscomfortValue { get; set; }

		/// <summary>
		///     "Anzahl Übereinstimmungen" with searched effects.
		/// </summary>
		/// <author>Anna Krebs</author>
		public int MatchAmount { get; set; }

		/// <summary>
		///     All the effects, that an essential oil is effective for as one string separated with ;@
		/// </summary>
		/// <author>Anna Krebs</author>
		public string SearchEffectTextsInEssentialOil { get; set; }
	}
}