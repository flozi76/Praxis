using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;
using log4net;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for "Ätherische Öle".
    /// Basic functionality is done in Service.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilService : Service<EssentialOil, EssentialOilFilter, IEssentialOilRepository>, IEssentialOilService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEssentialOilRepository _essentialOilRepository;

        private readonly IEssentialOilEffectRepository _essentialOilEffectRepository;

        private readonly IEssentialOilMoleculeRepository _essentialOilMoleculeRepository;

        private readonly IEffectRepository _effectRepository;

        private readonly IMoleculeRepository _moleculeRepository;

        public EssentialOilService(IEssentialOilRepository essentialOilRepository, IEssentialOilEffectRepository essentialOilEffectRepository, IEssentialOilMoleculeRepository essentialOilMoleculeRepository, IEffectRepository effectRepository, IMoleculeRepository moleculeRepository)
            : base(essentialOilRepository)
        {
            _essentialOilRepository = essentialOilRepository;
            _essentialOilEffectRepository = essentialOilEffectRepository;
            _essentialOilMoleculeRepository = essentialOilMoleculeRepository;
            _effectRepository = effectRepository;
            _moleculeRepository = moleculeRepository;
        }

        /// <summary>
        /// Deletes essential oil, the assigned molecules and effects in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="essentialOilId"></param>
        public async Task<ValidationResultList> DeleteEssentialOilWithAssignmentsAsync(string essentialOilId)
        {
            ValidationResultList validationResult = await Repository.DeleteAsync(essentialOilId);

            // Only delete assigned values, if deletion of essential oil was successful.
            if (!validationResult.HasErrors)
            {
                // Get all assignments between the essential oil and the molecules.
                IList<EssentialOilMolecule> essentialOilMolecules = await _essentialOilMoleculeRepository.GetByFilterAsync(new EssentialOilMoleculeFilter { EssentialOilId = essentialOilId });
                foreach (EssentialOilMolecule essentialOilMolecule in essentialOilMolecules)
                {
                    // Delete the assignments between the essential oil and the molecule.
                    validationResult = await _essentialOilMoleculeRepository.DeleteAsync(essentialOilMolecule.Id);
                }

                // Get all assignments between the essential oil and the effects.
                IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectRepository.GetByFilterAsync(new EssentialOilEffectFilter {EssentialOilId = essentialOilId});
                foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
                {
                    // Delete the assignments between the essential oil and the effect.
                    validationResult = await _essentialOilEffectRepository.DeleteAsync(essentialOilEffect.Id);
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Gets all molecules, that are assigned to a specific essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="essentialOilId"></param>
        /// <returns></returns>
        public async Task<IList<Molecule>> GetAssignedMoleculesForEssentialOilAsync(string essentialOilId)
        {
            // Get ids of assigned molecules.
            IList<EssentialOilMolecule> essentialOilMolecules = await _essentialOilMoleculeRepository.GetByFilterAsync(new EssentialOilMoleculeFilter { EssentialOilId = essentialOilId });

            List<Molecule> molecules = new List<Molecule>();
            foreach (EssentialOilMolecule essentialOilMolecule in essentialOilMolecules)
            {
                // Get assigned molecules.
                Molecule molecule = await _moleculeRepository.GetByIdAsync(essentialOilMolecule.MoleculeId);
                molecule.MoleculePercentage = essentialOilMolecule.MoleculePercentage;

                molecules.Add(molecule);
            }

            return molecules;
        }

        /// <summary>
        /// Gets all effects, that are assigned to a specific essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="essentialOilId"></param>
        /// <returns></returns>
        public async Task<IList<Effect>> GetAssignedEffectsForEssentialOilAsync(string essentialOilId)
        {
            // Get ids of assigned effects.
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectRepository.GetByFilterAsync(new EssentialOilEffectFilter { EssentialOilId = essentialOilId });

            List<Effect> effects = new List<Effect>();
            foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
            {
                // Get assigned effects.
                Effect effect = await _effectRepository.GetByIdAsync(essentialOilEffect.EffectId);
                effect.EffectDegree = essentialOilEffect.EffectDegree;

                effects.Add(effect);
            }

            return effects;
        }

        /// <summary>
        /// Gets the result list of essential oil according to the searched effects.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="searchedEffects">The search text and the discomfort value (Beschwerdeausmass).</param>
        /// <returns></returns>
        public async Task<IList<SearchEssentialOilItem>> GetEssentialOilResultsBySearchedEffectsNameAsync(IList<SearchEffectItem> searchedEffects)
        {
            // List with all the essential oils of all searched effects. Is the result list.
            List<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();

            // Do this for all searched effects.
            foreach (SearchEffectItem searchedEffect in searchedEffects)
            {
                // Don't search for empty search values.
                if (!string.IsNullOrWhiteSpace(searchedEffect.SearchEffectText) &&  searchedEffect.DiscomfortValue > 0)
                {
                    IList<EssentialOilEffect> essentialOilEffects = await GetEssentialOilsForSearchedEffectAsync(searchedEffect);
                    Log.Info($"Search result of essential oil names '{string.Join(", ", searchEssentialOilItemsResults.Select(e => e.EssentialOil.Name))}' where found.");

                    // Only create SearchEssentialOilItem if essential oils are assigned to the searched effect.
                    if (essentialOilEffects.Count != 0)
                    {
                        // List that contains all essential oils that are assigned to one specific searched effect.
                        List<SearchEssentialOilItem> searchEssentialOilItems = new List<SearchEssentialOilItem>();
                        await CreateSearchEssentialOilItemsListAsync(searchEssentialOilItems, essentialOilEffects, searchedEffect);

                        JoinResult(searchEssentialOilItemsResults, searchEssentialOilItems);
                    }
                }
            }

            // Return all found essential oils for searched effects.
            return searchEssentialOilItemsResults;
        }

        /// <summary>
        /// Creates SearchEssentialOilItemResult list.
        /// Checks if essential oil was already added to result. If so, recalculate the EffectDegreeDiscomfortValue.
        /// Otherwise add searchEssentialOilItem to result list.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="searchEssentialOilItemsResults"></param>
        /// <param name="searchEssentialOilItems"></param>
        private void JoinResult(List<SearchEssentialOilItem> searchEssentialOilItemsResults, List<SearchEssentialOilItem> searchEssentialOilItems)
        {
            // Add the search item to the result list, that contains all essential oils.
            foreach (SearchEssentialOilItem searchEssentialOilItem in searchEssentialOilItems)
            {
                // Check if search essential oil item already exists in result list.
                SearchEssentialOilItem searchEssentialOilItemInResultList = searchEssentialOilItemsResults.FirstOrDefault(s => s.EssentialOilId == searchEssentialOilItem.EssentialOilId);

                // Only add to result list, if essential oil with this id has not yet been added to the result list.
                // Otherwise only recalculate the EffectDegreeDiscomfortValue anew
                if (searchEssentialOilItemInResultList == null)
                {
                    // Is new essential oil.
                    searchEssentialOilItem.MatchAmount = 1;
                    searchEssentialOilItem.SearchEffectTextsInEssentialOil = searchEssentialOilItem.SearchEffectTextsInEssentialOil.Trim();
                    searchEssentialOilItemsResults.Add(searchEssentialOilItem);

                    Log.Info($"Add new search essential oil item {searchEssentialOilItem.EssentialOil.Name} with id {searchEssentialOilItem.EssentialOilId} and with product of effect degree * discomfort value of {searchEssentialOilItem.EffectDegreeDiscomfortValue} to result list.");
                }
                else
                {
                    // Recalculate EffectDegreeDiscomfortValue anew, if essential oil has already been added to result list.
                    searchEssentialOilItemInResultList.EffectDegreeDiscomfortValue = searchEssentialOilItemInResultList.EffectDegreeDiscomfortValue + searchEssentialOilItem.EffectDegreeDiscomfortValue;
                    searchEssentialOilItemInResultList.MatchAmount = searchEssentialOilItemInResultList.MatchAmount + 1;

                    // Concatenate all effects, that the essential oil is effective for as one string separated with ;@
                    searchEssentialOilItemInResultList.SearchEffectTextsInEssentialOil =  $"{searchEssentialOilItemInResultList.SearchEffectTextsInEssentialOil};@{searchEssentialOilItem.SearchEffectTextsInEssentialOil.Trim()}";

                    Log.Info($"Essential oil item {searchEssentialOilItem.EssentialOil.Name} with id {searchEssentialOilItem.EssentialOilId} already exist in result list. Recalculate the the product of effect degree * discomfort value to {searchEssentialOilItemInResultList.EffectDegreeDiscomfortValue}.");
                }
            }
        }

        /// <summary>
        /// Add every essential oil to list, that is assigned for specific search effect.
        /// Calculate the EffectDegreeDiscomfortValue
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="searchEssentialOilItems"></param>
        /// <param name="essentialOilEffects"></param>
        /// <param name="searchedEffect"></param>
        /// <returns></returns>
        private async Task CreateSearchEssentialOilItemsListAsync(List<SearchEssentialOilItem> searchEssentialOilItems, IList<EssentialOilEffect> essentialOilEffects, SearchEffectItem searchedEffect)
        {
            foreach (EssentialOilEffect essentialOilEffect in essentialOilEffects)
            {
                // Get the essential oils according to the effect they are assigned to.
                EssentialOil essentialOil = await Repository.GetByIdAsync(essentialOilEffect.EssentialOilId);

                // Create search essential oil item for each assigned essential oil that is assigned to a specific effect.
                SearchEssentialOilItem searchEssentialOilItem = new SearchEssentialOilItem();
                searchEssentialOilItem.EssentialOil = essentialOil;
                searchEssentialOilItem.EssentialOilId = essentialOil.Id;
                searchEssentialOilItem.SearchEffectTextsInEssentialOil = searchedEffect.SearchEffectText;

                // Get the product of "Wirksamkeit" of effect and "Beschwerdeausmass" of each searched effect.
                searchEssentialOilItem.EffectDegreeDiscomfortValue = essentialOilEffect.EffectDegree * searchedEffect.DiscomfortValue;

                // Add the essential oil to the list for the specific effect.
                searchEssentialOilItems.Add(searchEssentialOilItem);

                Log.Info($"Essential oil {searchEssentialOilItem.EssentialOil.Name} with id {searchEssentialOilItem.EssentialOilId} was found for effect with id {essentialOilEffect.EffectId} and effect degree {essentialOilEffect.EffectDegree}.");
            }
        }

        /// <summary>
        /// Gets all the essential oils for specific searched effect.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="searchedEffect"></param>
        /// <returns></returns>
        private async Task<IList<EssentialOilEffect>> GetEssentialOilsForSearchedEffectAsync(SearchEffectItem searchedEffect)
        {
            Log.Info($"Searching for effect with search text {searchedEffect.SearchEffectText} and discomfort value {searchedEffect.DiscomfortValue}");

            // Get effect by search text.
            IList<Effect> effects = await _effectRepository.GetByFilterAsync(new EffectFilter { Name = searchedEffect.SearchEffectText });

            // Return empty list if no effect was found.
            if (effects.Count == 0)
            {
                return new List<EssentialOilEffect>();
            }

            Effect effect = effects.SingleOrDefault();
            if (effect == null)
            {
                Log.Error($"No effect for search text {searchedEffect.SearchEffectText} was found.");
                throw new ArgumentNullException($"{Resources.Resources.Error_UnexpectedError} {string.Format(Resources.Resources.Error_NoEffectFoundForSearchText, searchedEffect.SearchEffectText)}");
            }
            Log.Info($"Effect {effect.Name} with id {effect.Id} was found for searched text.");

            // Get essential oils that are assigned to searched effect.
            IList<EssentialOilEffect> essentialOilEffects = await _essentialOilEffectRepository.GetByFilterAsync(new EssentialOilEffectFilter { EffectId = effect.Id });
            return essentialOilEffects;
        }

        /// <summary>
        /// Calculates the Gewichtete Übereinstimmung.
        /// E.g. 3 Sliders with inputted DiscomfortValue 4, 1, 2.
        /// MaxEffectDegreeDiscomfortValue would be (4 * 4) + (1 * 4) + (2 * 4) = 28
        /// EffectDegreeDiscomfortValue for oil with 3 matches and EffectDegree 3, 2, 4 would be (4 * 3) + (1 * 2) + (2 * 4) = 22
        /// WeightedMatchValue for oil would be 100 / 28 * 22 = 79
        /// EffectDegreeDiscomfortValue for oil with 2 matches and EffectDegree 2, 2 would be (4 * 2) + (1 * 2) = 10
        /// WeightedMatchValue for oil would be 100 / 28 * 10 = 36
        /// EffectDegreeDiscomfortValue for oil with 1 match and EffectDegree 4 would be (4 * 4) = 16
        /// WeightedMatchValue for oil would be 100 / 28 * 16 = 57
        /// EffectDegreeDiscomfortValue for another oil with 1 match and EffectDegree 3 would be (1 * 3) = 3
        /// WeightedMatchValue for oil would be 100 / 28 * 3 = 11
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="resultItem"></param>
        /// <param name="maxEffectDegreeDiscomfortValue"></param>
        /// <returns></returns>
        public int CalculateWeightedMatchValue(SearchEssentialOilItem resultItem, int maxEffectDegreeDiscomfortValue)
        {
            // Calculate the weightedMatchValue in order to set the  progress bar.
            int weightedMatchValue;
            if (maxEffectDegreeDiscomfortValue != 0)
            {
                // E.g. 100 / 28 * 22 = 79
                double weightedMatchValueDouble = (double)100 / maxEffectDegreeDiscomfortValue * resultItem.EffectDegreeDiscomfortValue;
                weightedMatchValue = Convert.ToInt32(weightedMatchValueDouble);
            }
            else
            {
                Log.Error($"An unexpected error occurred while CalculateWeightedMatchValue. MaxEffectDegreeDiscomfortValue is 0. Cannot divide by zero.");
                throw new DivideByZeroException(string.Format(Resources.Resources.Error_UnexpectedError, Resources.Resources.Error_MaxEffectDegreeDiscomfortValueIsZero, resultItem.EssentialOil.Id));
            }

            Log.Info($"CalculateWeightedMatchValue 100 /  maxEffectDegreeDiscomfortValue  * resultItem.EffectDegreeDiscomfortValue = weightedMatchValue -> 100 / {maxEffectDegreeDiscomfortValue} * {resultItem.EffectDegreeDiscomfortValue} = {weightedMatchValue} for essential oil item {resultItem.EssentialOil.Name} with id {resultItem.EssentialOilId}.");

            return weightedMatchValue;
        }
    }
}

