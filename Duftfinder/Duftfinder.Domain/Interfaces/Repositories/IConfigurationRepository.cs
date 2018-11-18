using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Represents the interface of the store for Configurations.
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IConfigurationRepository : IRepository<Configuration, ConfigurationFilter>
    {
    }
}
