namespace Duftfinder.Domain.Filters
{
	/// <summary>
	///     "Moleküle" for "Ätherisches Öl" filter, for filtering for specific properties, sorting etc.
	///     Important: Properties have to be nullable, in order to filter properly.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilMoleculeFilter : Filter
	{
		public string EssentialOilId { get; set; }

		public string MoleculeId { get; set; }
	}
}