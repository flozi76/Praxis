using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using log4net;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for "Wirkungen" for "Ätherisches Öl".
	///     Basic functionality is done in Service.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilEffectService :
		Service<EssentialOilEffect, EssentialOilEffectFilter, IEssentialOilEffectRepository>, IEssentialOilEffectService
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IEssentialOilEffectRepository _essentialOilEffectRepository;

		public EssentialOilEffectService(IEssentialOilEffectRepository essentialOilEffectRepository)
			: base(essentialOilEffectRepository)
		{
			_essentialOilEffectRepository = essentialOilEffectRepository;
		}

		/// <summary>
		///     Deletes all assigned essential oils for specific effect from database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="effectId"></param>
		/// <returns></returns>
		public async Task<ValidationResultList> DeleteAssignedEssentialOilsAsync(string effectId)
		{
			var validationResult = new ValidationResultList();

			var essentialOilEffects =
				await _essentialOilEffectRepository.GetAllAsync(new EssentialOilEffectFilter {EffectId = effectId});

			foreach (var essentialOilEffect in essentialOilEffects)
			{
				validationResult = await _essentialOilEffectRepository.DeleteAsync(essentialOilEffect.Id);
				Log.Info(
					$"Delete assigned essentialOil with id {essentialOilEffect.EssentialOilId} for effect with id {essentialOilEffect.EffectId}");
			}

			return validationResult;
		}

		/// <summary>
		///     Deletes all assigned effects for specific essential oil from database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="essentialOilId"></param>
		/// <returns></returns>
		public async Task<ValidationResultList> DeleteAssignedEffectsAsync(string essentialOilId)
		{
			var validationResult = new ValidationResultList();

			var essentialOilEffects = await _essentialOilEffectRepository.GetAllAsync(new EssentialOilEffectFilter
				{EssentialOilId = essentialOilId});

			foreach (var essentialOilEffect in essentialOilEffects)
			{
				validationResult = await _essentialOilEffectRepository.DeleteAsync(essentialOilEffect.Id);
				Log.Info(
					$"Delete assigned effect with id {essentialOilEffect.EffectId} for essential oil with id {essentialOilEffect.EssentialOilId}");
			}

			return validationResult;
		}
	}
}