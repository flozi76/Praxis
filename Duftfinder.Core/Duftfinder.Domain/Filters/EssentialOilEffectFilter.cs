namespace Duftfinder.Domain.Filters
{
	/// <summary>
	///     "Wirkungen" for "Ätherisches Öl" filter, for filtering for specific properties, sorting etc.
	///     Important: Properties have to be nullable, in order to filter properly.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilEffectFilter : Filter
	{
		public string EssentialOilId { get; set; }

		public string EffectId { get; set; }
	}
}