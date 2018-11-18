using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Database.Repositories;
using Duftfinder.Domain.Helpers;
using log4net;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for "Moleküle" for "Wirkung".
    /// Basic functionality is done in Service.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectMoleculeService : Service<EffectMolecule, EffectMoleculeFilter, IEffectMoleculeRepository>, IEffectMoleculeService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEffectMoleculeRepository _effectMoleculeRepository;

        public EffectMoleculeService(IEffectMoleculeRepository effectMoleculeRepository) 
            : base(effectMoleculeRepository)
        {
            _effectMoleculeRepository = effectMoleculeRepository;
        }

        /// <summary>
        /// Deletes all assigned molecules for specific effect from database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="effectId"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> DeleteAssignedMoleculesAsync(string effectId)
        {
            ValidationResultList validationResult = new ValidationResultList();

            IList<EffectMolecule> effectMolecules = await _effectMoleculeRepository.GetAllAsync(new EffectMoleculeFilter { EffectId = effectId });

            foreach (EffectMolecule effectMolecule in effectMolecules)
            {
                validationResult = await _effectMoleculeRepository.DeleteAsync(effectMolecule.Id);
                Log.Info($"Delete assigned molecule with id {effectMolecule.MoleculeId} for effect with id {effectMolecule.EffectId}");
            }

            return validationResult;
        }
    }
}
