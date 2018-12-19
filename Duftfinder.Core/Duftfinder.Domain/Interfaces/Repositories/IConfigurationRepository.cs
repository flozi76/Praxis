using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
	/// <summary>
	///     Represents the interface of the store for Configurations.
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IConfigurationRepository : IRepository<Configuration, ConfigurationFilter>
	{
	}
}