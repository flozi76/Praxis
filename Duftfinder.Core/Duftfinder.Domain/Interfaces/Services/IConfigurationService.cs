using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service of Configuration.
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IConfigurationService : IService<Configuration, ConfigurationFilter>
	{
		Task<string> GetConfigurationParameterByKeyAsync(string key);
	}
}