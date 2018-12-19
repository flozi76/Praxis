using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for "Molekül".
	///     Basic functionality is done in Service.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class MoleculeService : Service<Molecule, MoleculeFilter, IMoleculeRepository>, IMoleculeService
	{
		private readonly IEffectMoleculeRepository _effectMoleculeRepository;

		private readonly IEssentialOilMoleculeRepository _essentialOilMoleculeRepository;
		private readonly IMoleculeRepository _moleculeRepository;

		private readonly ISubstanceRepository _substanceRepository;

		public MoleculeService(IMoleculeRepository moleculeRepository,
			IEssentialOilMoleculeRepository essentialOilMoleculeRepository,
			IEffectMoleculeRepository effectMoleculeRepository, ISubstanceRepository substanceRepository)
			: base(moleculeRepository)
		{
			_moleculeRepository = moleculeRepository;
			_essentialOilMoleculeRepository = essentialOilMoleculeRepository;
			_effectMoleculeRepository = effectMoleculeRepository;
			_substanceRepository = substanceRepository;
		}

		/// <summary>
		///     Deletes molecule, the assigned essential oils and effects in database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="moleculeId"></param>
		public async Task<ValidationResultList> DeleteMoleculeWithAssignmentsAsync(string moleculeId)
		{
			var validationResult = await Repository.DeleteAsync(moleculeId);

			// Only delete assigned values, if deletion of molecule was successful.
			if (!validationResult.HasErrors)
			{
				// Get all assignments between the molecule and the essential oils.
				var essentialOilMolecules =
					await _essentialOilMoleculeRepository.GetByFilterAsync(new EssentialOilMoleculeFilter
						{MoleculeId = moleculeId});
				foreach (var essentialOilMolecule in essentialOilMolecules)
					// Delete the assignments between the essential oil and the molecule.
					validationResult = await _essentialOilMoleculeRepository.DeleteAsync(essentialOilMolecule.Id);

				// Get all assignments between the molecule and the effects.
				var effectMolecules =
					await _effectMoleculeRepository.GetByFilterAsync(new EffectMoleculeFilter
						{MoleculeId = moleculeId});
				foreach (var effectMolecule in effectMolecules)
					// Delete the assignments between the effect and the molecule.
					validationResult = await _effectMoleculeRepository.DeleteAsync(effectMolecule.Id);
			}

			return validationResult;
		}

		/// <summary>
		///     Gets the substance for specific molecule.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="substanceId"></param>
		/// <returns></returns>
		public async Task<Substance> GetSubstanceForMoleculeAsync(string substanceId)
		{
			var substance = await _substanceRepository.GetByIdAsync(substanceId);

			return substance;
		}
	}
}