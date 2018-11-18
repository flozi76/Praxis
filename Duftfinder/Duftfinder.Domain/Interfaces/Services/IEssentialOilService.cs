using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service of "Ätherische Öle".
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IEssentialOilService : IService<EssentialOil, EssentialOilFilter>
    {
        Task<ValidationResultList> DeleteEssentialOilWithAssignmentsAsync(string essentialOilId);
        
        Task<IList<Molecule>> GetAssignedMoleculesForEssentialOilAsync(string essentialOilId);
        
        Task<IList<Effect>> GetAssignedEffectsForEssentialOilAsync(string essentialOilId);
        
        Task<IList<SearchEssentialOilItem>> GetEssentialOilResultsBySearchedEffectsNameAsync(IList<SearchEffectItem> searchedEffects);

        int CalculateWeightedMatchValue(SearchEssentialOilItem resultItem, int maxEffectDegreeDiscomfortValue);
    }
}
