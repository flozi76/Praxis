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
    /// Entity for "Wirkungen" for "Ätherisches Öl"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilEffect : Entity
    {
        public string EssentialOilId { get; set; }

        public string EffectId { get; set; }

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
