using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for configuration related stuff.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class ConfigurationService : Service<Configuration, ConfigurationFilter, IConfigurationRepository>,
		IConfigurationService
	{
		private readonly IConfigurationRepository _configurationRepository;

		public ConfigurationService(IConfigurationRepository configurationRepository)
			: base(configurationRepository)
		{
			_configurationRepository = configurationRepository;
		}

		/// <summary>
		///     Gets the configuration parameter value by its key.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="key"></param>
		/// <returns></returns>
		public async Task<string> GetConfigurationParameterByKeyAsync(string key)
		{
			var configurationParameters =
				await _configurationRepository.GetByFilterAsync(new ConfigurationFilter {Key = key});
			return configurationParameters.SingleOrDefault()?.Value;
		}
	}
}