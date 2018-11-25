using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Duftfinder.Domain.Enums
{
    /// <summary>
    /// Enum of Categories.
    /// </summary>
    /// <author>Anna Krebs</author>
    public enum CategoryValue
    {
        [Display(Name = nameof(Resources.Resources.CategoryValue_Pain), ResourceType = typeof(Resources.Resources))]
        Pain = 1,

        [Display(Name = nameof(Resources.Resources.CategoryValue_Muscle), ResourceType = typeof(Resources.Resources))]
        Muscle = 2,

        [Display(Name = nameof(Resources.Resources.CategoryValue_SkinAndScars), ResourceType = typeof(Resources.Resources))]
        SkinAndScars = 3,

        [Display(Name = nameof(Resources.Resources.CategoryValue_Tissue), ResourceType = typeof(Resources.Resources))]
        Tissue = 4,

        [Display(Name = nameof(Resources.Resources.CategoryValue_BloodCirculation), ResourceType = typeof(Resources.Resources))]
        BloodCirculation = 5,

        [Display(Name = nameof(Resources.Resources.CategoryValue_NervousSystemGeneral), ResourceType = typeof(Resources.Resources))]
        NervousSystemGeneral = 6,

        [Display(Name = nameof(Resources.Resources.CategoryValue_NervousSystemNeurotransmitter), ResourceType = typeof(Resources.Resources))]
        NervousSystemNeurotransmitter = 7,

        [Display(Name = nameof(Resources.Resources.CategoryValue_EntericNervousSystem), ResourceType = typeof(Resources.Resources))]
        EntericNervousSystem = 8,

        [Display(Name = nameof(Resources.Resources.CategoryValue_EndocrineHormones), ResourceType = typeof(Resources.Resources))]
        EndocrineHormones = 9,

        [Display(Name = nameof(Resources.Resources.CategoryValue_ImmuneSystem), ResourceType = typeof(Resources.Resources))]
        ImmuneSystem = 10,

        [Display(Name = nameof(Resources.Resources.CategoryValue_Inflammation), ResourceType = typeof(Resources.Resources))]
        Inflammation = 11,

        [Display(Name = nameof(Resources.Resources.CategoryValue_IntegrationYoungerSelf), ResourceType = typeof(Resources.Resources))]
        IntegrationYoungerSelf = 12,

        [Display(Name = nameof(Resources.Resources.CategoryValue_OrderInCircle), ResourceType = typeof(Resources.Resources))]
        OrderInCircle = 13,

        [Display(Name = nameof(Resources.Resources.CategoryValue_AccessToInnerStrength), ResourceType = typeof(Resources.Resources))]
        AccessToInnerStrength = 14,

        [Display(Name = nameof(Resources.Resources.CategoryValue_BlockageDissolving), ResourceType = typeof(Resources.Resources))]
        BlockageDissolving = 15,

        [Display(Name = nameof(Resources.Resources.CategoryValue_AchievingGoalsAttention), ResourceType = typeof(Resources.Resources))]
        AchievingGoalsAttention = 16,

        [Display(Name = nameof(Resources.Resources.CategoryValue_AchievingGoalsActivationLevel), ResourceType = typeof(Resources.Resources))]
        AchievingGoalsActivationLevel = 17,

        [Display(Name = nameof(Resources.Resources.CategoryValue_AchievingGoalsNerves), ResourceType = typeof(Resources.Resources))]
        AchievingGoalsNerves = 18,

        [Display(Name = nameof(Resources.Resources.CategoryValue_MeridiansAttackYinYang), ResourceType = typeof(Resources.Resources))]
        MeridiansAttackYinYang = 19,
    }
}