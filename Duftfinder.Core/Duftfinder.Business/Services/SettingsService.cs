using System.Threading.Tasks;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for settings related stuff.
	///     E.g. initializing the database.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class SettingsService : ISettingsService
	{
		private readonly ISettingsRepository _settingsRepository;

		public SettingsService(ISettingsRepository settingsRepository)
		{
			_settingsRepository = settingsRepository;
		}

		public async Task InitializeSubstancesAndCategoriesAsync()
		{
			await _settingsRepository.InitializeSubstancesAndCategoriesAsync();
		}

		public async Task InitializeEssentialOilsAsync()
		{
			await _settingsRepository.InitializeEssentialOilsAsync();
		}

		public async Task InitializeEffectsAsync()
		{
			await _settingsRepository.InitializeEffectsAsync();
		}

		public async Task InitializeMoleculesAsync()
		{
			await _settingsRepository.InitializeMoleuclesAsync();
		}

		public async Task InitializeUsersAsync()
		{
			await _settingsRepository.InitializeUsersAsync();
		}

		public async Task InitializeConfigurationValuesAsync()
		{
			await _settingsRepository.InitializeConfigurationValuesAsync();
		}
	}
}