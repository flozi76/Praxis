namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for Role.
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class Role : Entity
    {
        public string Name { get; set; }


        public override string GetPrimaryName()
        {
            return nameof(Name);
        }

        public override string GetPrimaryValue()
        {
            return nameof(Name);
        }
    }
}
