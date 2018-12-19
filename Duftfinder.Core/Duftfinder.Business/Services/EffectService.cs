using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for "Wirkung".
	///     Basic functionality is done in Service.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EffectService : Service<Effect, EffectFilter, IEffectRepository>, IEffectService
	{
		private readonly IEffectMoleculeRepository _effectMoleculeRepository;
		private readonly IEffectRepository _effectRepository;

		private readonly IEssentialOilEffectRepository _essentialOilEffectRepository;

		public EffectService(IEffectRepository effectRepository,
			IEssentialOilEffectRepository essentialOilEffectRepository,
			IEffectMoleculeRepository effectMoleculeRepository)
			: base(effectRepository)
		{
			_effectRepository = effectRepository;
			_essentialOilEffectRepository = essentialOilEffectRepository;
			_effectMoleculeRepository = effectMoleculeRepository;
		}

		/// <summary>
		///     Deletes effect, the assigned molecules and essential oils in database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="effectId"></param>
		public async Task<ValidationResultList> DeleteEffectWithAssignmentsAsync(string effectId)
		{
			var validationResult = await Repository.DeleteAsync(effectId);

			// Only delete assigned values, if deletion of effect was successful.
			if (!validationResult.HasErrors)
			{
				// Get all assignments between the effect and the molecules.
				var effectMolecules =
					await _effectMoleculeRepository.GetByFilterAsync(new EffectMoleculeFilter {EffectId = effectId});
				foreach (var effectMolecule in effectMolecules)
					// Delete the assignments between the effect and the molecule.
					validationResult = await _effectMoleculeRepository.DeleteAsync(effectMolecule.Id);

				// Get all assignments between the effect and the essential oils.
				var essentialOilEffects =
					await _essentialOilEffectRepository.GetByFilterAsync(new EssentialOilEffectFilter
						{EffectId = effectId});
				foreach (var essentialOilEffect in essentialOilEffects)
					// Delete the assignments between the effect and the essential oil.
					validationResult = await _essentialOilEffectRepository.DeleteAsync(essentialOilEffect.Id);
			}

			return validationResult;
		}
	}
}