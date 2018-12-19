namespace Duftfinder.Domain.Entities
{
	/// <summary>
	///     Entity for "Stoffklasse"
	///     Bson attributes have to match attributes in mongoDb.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class Substance : Entity
	{
		public string Name { get; set; }

		public int SortOrder { get; set; }

		public override string GetPrimaryName()
		{
			return nameof(Name);
		}

		public override string GetPrimaryValue()
		{
			return Name;
		}
	}
}