using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Duftfinder.Domain.Entities
{
	/// <summary>
	///     Entity for "Molekül"
	///     Bson attributes have to match attributes in mongoDb.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class Molecule : Entity
	{
		public string Name { get; set; }

		public bool IsGeneral { get; set; }

		public ObjectId SubstanceId { get; set; }

		[BsonIgnore] public double MoleculePercentage { get; set; }

		/// <summary>
		///     Map Id to string, in order to use irrespective of
		///     MongoDB in web project.
		///     Return null, if new ObjectId.
		/// </summary>
		/// <author>Anna Krebs</author>
		[BsonIgnore]
		public string SubstanceIdString
		{
			get
			{
				if (SubstanceId != ObjectId.Empty) return SubstanceId.ToString();
				return null;
			}
			set
			{
				if (value != null) SubstanceId = new ObjectId(value);
			}
		}

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