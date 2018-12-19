namespace Duftfinder.Domain.Filters
{
	/// <summary>
	///     "Moleküle" for "Wirkung" filter, for filtering for specific properties, sorting etc.
	///     Important: Properties have to be nullable, in order to filter properly.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EffectMoleculeFilter : Filter
	{
		public string EffectId { get; set; }

		public string MoleculeId { get; set; }
	}
}