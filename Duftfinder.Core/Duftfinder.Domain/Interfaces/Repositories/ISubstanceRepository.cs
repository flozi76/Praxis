using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
	/// <summary>
	///     Represents the interface of the store for "Stoffklasse".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface ISubstanceRepository : IRepository<Substance, SubstanceFilter>
	{
	}
}