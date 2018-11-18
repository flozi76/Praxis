using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service of "Stoffklasse".
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface ISubstanceService : IService<Substance, SubstanceFilter>
    {
    }
}
