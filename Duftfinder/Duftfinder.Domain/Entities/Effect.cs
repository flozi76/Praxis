using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for "Wirkung"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>

    public class Effect : Entity
    {
        public string Name { get; set; }

        public string Details { get; set; }

        public ObjectId CategoryId { get; set; }

        [BsonIgnore]
        public int EffectDegree { get; set; }

        /// <summary>
        /// Map Id to string, in order to use irrespective of 
        /// MongoDB in web project.
        /// Return null, if new ObjectId. 
        /// </summary>
        /// <author>Anna Krebs</author>
        [BsonIgnore]
        public string CategoryIdString
        {
            get
            {
                if (CategoryId != ObjectId.Empty)
                {
                    return CategoryId.ToString();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    CategoryId = new ObjectId(value);
                }
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
