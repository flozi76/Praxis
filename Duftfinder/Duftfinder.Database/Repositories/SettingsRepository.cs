using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using log4net;


namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of settings related stuff.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class SettingsRepository : ISettingsRepository
    {
        private readonly MongoContext _dbContext;

        private readonly MongoDataInitializer _dataInitializer;

        public SettingsRepository(MongoContext context)
        {
            _dbContext = context;
            _dataInitializer = new MongoDataInitializer(_dbContext);
        }

        public async Task InitializeSubstancesAndCategoriesAsync()
        {
            await _dataInitializer.InitSubstancesAndCategories();
        }

        public async Task InitializeEssentialOilsAsync()
        {
            await _dataInitializer.InitEssentialOils();
        }

        public async Task InitializeEffectsAsync()
        {
            await _dataInitializer.InitEffects();
        }

        public async Task InitializeMoleuclesAsync()
        {
            await _dataInitializer.InitMolecules();
        }

        public async Task InitializeUsersAsync()
        {
            await _dataInitializer.InitUsersAndRoles();
        }

        public async Task InitializeConfigurationValuesAsync()
        {
            await _dataInitializer.InitConfigurationValues();
        }
    }
}
