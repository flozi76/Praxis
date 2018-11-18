using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for Configuration.
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class Configuration : Entity
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }


        public override string GetPrimaryName()
        {
            return nameof(Key);
        }

        public override string GetPrimaryValue()
        {
            return nameof(Key);
        }
    }
}
