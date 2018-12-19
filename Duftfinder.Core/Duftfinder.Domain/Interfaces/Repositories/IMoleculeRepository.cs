using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
	/// <summary>
	///     Represents the interface of the store for "Molekül".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IMoleculeRepository : IRepository<Molecule, MoleculeFilter>
	{
	}
}