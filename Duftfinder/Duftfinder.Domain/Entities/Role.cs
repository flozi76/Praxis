using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

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
