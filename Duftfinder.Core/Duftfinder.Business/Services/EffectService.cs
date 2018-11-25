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
    /// Contains business logic for "Wirkung".
    /// Basic functionality is done in Service.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectService : Service<Effect, EffectFilter, IEffectRepository>, IEffectService
    {
        private readonly IEffectRepository _effectRepository;

        private readonly IEssentialOilEffectRepository _essentialOilEffectRepository;

        private readonly IEffectMoleculeRepository _effectMoleculeRepository;

        public EffectService(IEffectRepository effectRepository, IEssentialOilEffectRepository essentialOilEffectRepository, IEffectMoleculeRepository effectMoleculeRepository) 
            : base(effectRepository)
        {
            _effectRepository = effectRepository;
            _essentialOilEffectRepository = essentialOilEffectRepository;
            _effectMoleculeRepository =  effectMoleculeRepository;
        }

        /// <summary>
        /// Deletes effect, the assigned molecules and essential oils in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="effectId"></param>
        public async Task<ValidationResultList> DeleteEffectWithAssignmentsAsync(string effectId)
        {
            ValidationResultList validationResult = await Repository.DeleteAsync(effectId);

            // Only delete assigned values, if deletion of effect was successful.
            if (!validationResult.HasErrors)
            {
                // Get all assignments between the effect and the molecules.
                IList<EffectMolecule> effectMolecules = await _effectMoleculeRepository.GetByFilterAsync(new EffectMoleculeFilter { EffectId = effectId });
                foreach (EffectMolecule effectMolecule in effectMolecules)
                {
                    // Delete the assignments between the effect and the molecule.
                    validationResult = await _effectMoleculeRepository.DeleteAsync(effectMolecule.Id);
                }

                // Get all assignments between the effect and the essential oils.
                IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectRepository.GetByFilterAsync(new EssentialOilEffectFilter { EffectId = effectId });
                foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
                {
                    // Delete the assignments between the effect and the essential oil.
                    validationResult = await _essentialOilEffectRepository.DeleteAsync(essentialOilEffect.Id);
                }
            }

            return validationResult;
        }
    }
}
