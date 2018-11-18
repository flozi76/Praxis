﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for "Wirkungskategorie"
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>

    public class Category : Entity
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
