using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Repositories
{
	/// <summary>
	///     Represents the interface of the store for "Moleküle" to"Ätherisches Öl".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IEssentialOilMoleculeRepository : IRepository<EssentialOilMolecule, EssentialOilMoleculeFilter>
	{
	}
}