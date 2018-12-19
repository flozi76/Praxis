using System.Threading.Tasks;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service for settings related stuff.
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface ISettingsService
	{
		Task InitializeSubstancesAndCategoriesAsync();

		Task InitializeEssentialOilsAsync();

		Task InitializeEffectsAsync();

		Task InitializeMoleculesAsync();

		Task InitializeUsersAsync();

		Task InitializeConfigurationValuesAsync();
	}
}