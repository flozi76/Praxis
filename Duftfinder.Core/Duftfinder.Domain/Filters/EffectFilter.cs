using System.Collections.Generic;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Filters
{
	/// <summary>
	///     "Wirkung" filter, for filtering for specific properties, sorting etc.
	///     Important: Properties have to be nullable, in order to filter properly.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EffectFilter : Filter
	{
		private static readonly Effect Effect = new Effect();

		public string Name { get; set; }

		/// <summary>
		///     Dictionary object for sorting entity.
		///     Key: Name of field in DB for sorting.
		///     Value: Sort direction. (Ascending or Descending)
		///     <author>Anna Krebs</author>
		/// </summary>
		public Dictionary<string, string> SortValues { get; set; } = new Dictionary<string, string>
		{
			{Effect.GetPrimaryName(), Constants.Ascending}
		};
	}
}