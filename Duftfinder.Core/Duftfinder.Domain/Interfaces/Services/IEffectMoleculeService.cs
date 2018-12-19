using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service of "Moleküle" for "Wirkung".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IEffectMoleculeService : IService<EffectMolecule, EffectMoleculeFilter>
	{
		Task<ValidationResultList> DeleteAssignedMoleculesAsync(string effectId);
	}
}