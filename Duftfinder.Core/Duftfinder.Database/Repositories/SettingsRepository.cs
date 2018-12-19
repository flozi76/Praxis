using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;

namespace Duftfinder.Database.Repositories
{
	/// <summary>
	///     Represents the store objects of settings related stuff.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class SettingsRepository : ISettingsRepository
	{
		private readonly MongoDataInitializer _dataInitializer;
		private readonly MongoContext _dbContext;

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