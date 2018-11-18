using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Duftfinder.Domain.Enums
{
    /// <summary>
    /// Enum of Essential Oil Types.
    /// </summary>
    /// <author>Anna Krebs</author>
    public enum EssentialOilType
    {
        [Display(Name = nameof(Resources.Resources.EssentialOilType_Oil), ResourceType = typeof(Resources.Resources))]
        Oil,

        [Display(Name = nameof(Resources.Resources.EssentialOilType_Hydrolat), ResourceType = typeof(Resources.Resources))]
        Hydrolat,

        [Display(Name = nameof(Resources.Resources.EssentialOilType_FatOil), ResourceType = typeof(Resources.Resources))]
        FatOil,
    }
}