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
    /// Represents the interface for the business logic service of "Wirkung".
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IEffectService : IService<Effect, EffectFilter>
    {
        Task<ValidationResultList> DeleteEffectWithAssignmentsAsync(string effectId);
    }
}
