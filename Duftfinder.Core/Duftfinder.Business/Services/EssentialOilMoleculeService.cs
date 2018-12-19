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
	///     Contains business logic for "Moleküle" for "Ätherisches Öl".
	///     Basic functionality is done in Service.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilMoleculeService :
		Service<EssentialOilMolecule, EssentialOilMoleculeFilter, IEssentialOilMoleculeRepository>,
		IEssentialOilMoleculeService
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IEssentialOilMoleculeRepository _essentialOilMoleculeRepository;

		public EssentialOilMoleculeService(IEssentialOilMoleculeRepository essentialOilMoleculeRepository)
			: base(essentialOilMoleculeRepository)
		{
			_essentialOilMoleculeRepository = essentialOilMoleculeRepository;
		}

		/// <summary>
		///     Deletes all assigned molecules for specific essential oil from database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="essentialOilId"></param>
		/// <returns></returns>
		public async Task<ValidationResultList> DeleteAssignedMoleculesAsync(string essentialOilId)
		{
			var validationResult = new ValidationResultList();

			var essentialOilMolecules = await _essentialOilMoleculeRepository.GetAllAsync(new EssentialOilMoleculeFilter
				{EssentialOilId = essentialOilId});

			foreach (var essentialOilMolecule in essentialOilMolecules)
			{
				validationResult = await _essentialOilMoleculeRepository.DeleteAsync(essentialOilMolecule.Id);
				Log.Info(
					$"Delete assigned molecule with id {essentialOilMolecule.MoleculeId} for essential oil with id {essentialOilMolecule.EssentialOilId}");
			}

			return validationResult;
		}
	}
}