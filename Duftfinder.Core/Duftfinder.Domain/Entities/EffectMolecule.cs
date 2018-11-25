namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for "Moleküle" for "Wirkung"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectMolecule : Entity
    {
        public string EffectId { get; set; }

        public string MoleculeId { get; set; }

        public int EffectDegree { get; set; }

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
