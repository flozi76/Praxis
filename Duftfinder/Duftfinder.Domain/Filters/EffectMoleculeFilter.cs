﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Filters
{
    /// <summary>
    /// "Moleküle" for "Wirkung" filter, for filtering for specific properties, sorting etc.
    /// Important: Properties have to be nullable, in order to filter properly.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectMoleculeFilter : Filter
    {
        public string EffectId { get; set; }

        public string MoleculeId { get; set; }
    }
}
