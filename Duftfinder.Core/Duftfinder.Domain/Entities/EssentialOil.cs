using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for "Ätherische Öle"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOil : Entity
    {
        public string Name { get; set; }

        public string NameLatin { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    
        public string PictureFileName { get; set; }

        public string PictureDataAsString { get; set; }

        public string PictureSource { get; set; }
        
        [BsonIgnore]
        public IList<Effect> Effects { get; set; }

        [BsonIgnore]
        public IList<Molecule> Molecules { get; set; }

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
