using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of "Moleküle" to "Wirkung".
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectMoleculeRepository : Repository<EffectMolecule, EffectMoleculeFilter>, IEffectMoleculeRepository
    {
        private readonly MongoContext _dbContext;

        public EffectMoleculeRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific Moleküle for Wirkung stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<EffectMolecule> ApplyFilter(EffectMoleculeFilter filter, IMongoCollection<EffectMolecule> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<EffectMolecule> bsonFilter = Builders<EffectMolecule>.Filter.Empty;
            List<FilterDefinition<EffectMolecule>> bsonFilterList = new List<FilterDefinition<EffectMolecule>> { bsonFilter };

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.EffectId))
            {
                // Filter exact match for EffectId.
                bsonFilter = Builders<EffectMolecule>.Filter.Eq(nameof(EffectMolecule.EffectId), filter.EffectId.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            if (!string.IsNullOrEmpty(filter.MoleculeId))
            {
                // Filter exact match for MoleculeId.
                bsonFilter = Builders<EffectMolecule>.Filter.Eq(nameof(EffectMolecule.MoleculeId), filter.MoleculeId.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<EffectMolecule> queryFilter = Builders<EffectMolecule>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Moleküle for Wirkung.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<EffectMolecule> ApplySorting(EffectMoleculeFilter filter, IMongoCollection<EffectMolecule> collection)
        {
            SortDefinitionBuilder<EffectMolecule> sortDefinitionBuilder = Builders<EffectMolecule>.Sort;
            List<SortDefinition<EffectMolecule>> sortDefinition = new List<SortDefinition<EffectMolecule>>();

            return sortDefinitionBuilder.Combine(sortDefinition);
        }
    }
}
