using System.Collections.Generic;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Filters
{
	/// <summary>
	///     "Ätherische Öle" filter, for filtering for specific properties, sorting etc.
	///     Important: Properties have to be nullable, in order to filter properly.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilFilter : Filter
	{
		private static readonly EssentialOil EssentialOil = new EssentialOil();

		public string Name { get; set; }

		/// <summary>
		///     The inputted search text for an essential oil.
		/// </summary>
		public string SearchText { get; set; }

		/// <summary>
		///     Dictionary object for sorting entity.
		///     Key: Name of field in DB for sorting.
		///     Value: Sort direction. (Ascending or Descending)
		///     <author>Anna Krebs</author>
		/// </summary>
		public Dictionary<string, string> SortValues { get; set; } = new Dictionary<string, string>
		{
			{EssentialOil.GetPrimaryName(), Constants.Ascending}
		};
	}
}