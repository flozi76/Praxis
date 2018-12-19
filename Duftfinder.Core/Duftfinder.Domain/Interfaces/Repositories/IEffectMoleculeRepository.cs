using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
	/// <summary>
	///     Represents the interface of the store for "Moleküle" to"Wirkung".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IEffectMoleculeRepository : IRepository<EffectMolecule, EffectMoleculeFilter>
	{
	}
}