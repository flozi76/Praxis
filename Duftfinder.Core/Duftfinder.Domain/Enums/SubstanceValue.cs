using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Duftfinder.Domain.Enums
{
    /// <summary>
    /// Enum of Substances.
    /// </summary>
    /// <author>Anna Krebs</author>
    public enum SubstanceValue
    {
        [Display(Name = nameof(Resources.Resources.SubstanceValue_AromaticAlcohol), ResourceType = typeof(Resources.Resources))]
        AromaticAlcohol = 14,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_AromaticAldehyde), ResourceType = typeof(Resources.Resources))]
        AromaticAldehyde = 13,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_AromaticEster), ResourceType = typeof(Resources.Resources))]
        AromaticEster = 11,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Cumarin), ResourceType = typeof(Resources.Resources))]
        Cumarin = 12,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Ester), ResourceType = typeof(Resources.Resources))]
        Ester = 6,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Ketone), ResourceType = typeof(Resources.Resources))]
        Ketone = 7,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Monoterpene), ResourceType = typeof(Resources.Resources))]
        Monoterpene = 1,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Monoterpenole), ResourceType = typeof(Resources.Resources))]
        Monoterpenole = 2,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Phenole), ResourceType = typeof(Resources.Resources))]
        Phenole = 9,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Aldehyde), ResourceType = typeof(Resources.Resources))]
        Aldehyde = 5,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Oxide), ResourceType = typeof(Resources.Resources))]
        Oxide = 8,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Phenylpropanderivate), ResourceType = typeof(Resources.Resources))]
        Phenylpropanderivate = 10,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_Sesquiterpene), ResourceType = typeof(Resources.Resources))]
        Sesquiterpene = 3,

        [Display(Name = nameof(Resources.Resources.SubstanceValue_SesquiterpenoleDiterpenole), ResourceType = typeof(Resources.Resources))]
        SesquiterpenoleDiterpenole = 4,
    }
}