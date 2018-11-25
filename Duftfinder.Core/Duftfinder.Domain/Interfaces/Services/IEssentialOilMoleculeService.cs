using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service of "Moleküle" for "Ätherisches Öl".
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IEssentialOilMoleculeService : IService<EssentialOilMolecule, EssentialOilMoleculeFilter>
    {
        Task<ValidationResultList> DeleteAssignedMoleculesAsync(string essentialOilId);
    }
}
