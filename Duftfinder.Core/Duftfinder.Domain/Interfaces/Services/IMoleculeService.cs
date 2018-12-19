using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service of "Molekül".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IMoleculeService : IService<Molecule, MoleculeFilter>
	{
		Task<ValidationResultList> DeleteMoleculeWithAssignmentsAsync(string moleculeId);

		Task<Substance> GetSubstanceForMoleculeAsync(string substanceId);
	}
}