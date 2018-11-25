using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duftfinder.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Represents the interface of the store for settings related stuff.
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface ISettingsRepository
    {
        Task InitializeSubstancesAndCategoriesAsync();

        Task InitializeEssentialOilsAsync();

        Task InitializeEffectsAsync();

        Task InitializeMoleuclesAsync();

        Task InitializeUsersAsync();

        Task InitializeConfigurationValuesAsync();
    }
}
