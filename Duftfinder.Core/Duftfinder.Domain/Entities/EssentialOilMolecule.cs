namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for "Moleküle" for "Ätherisches Öl"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilMolecule : Entity
    {
        public string EssentialOilId { get; set; }

        public string MoleculeId { get; set; }

        public double MoleculePercentage { get; set; }

        public override string GetPrimaryName()
        {
            return null;
        }

        public override string GetPrimaryValue()
        {
            return null;
        }
    }
}
