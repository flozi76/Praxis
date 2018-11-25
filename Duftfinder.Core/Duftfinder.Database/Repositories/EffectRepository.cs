using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using Duftfinder.Domain.Helpers;


namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of "Bilder".
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EffectRepository : Repository<Effect, EffectFilter>, IEffectRepository
    {
        private readonly MongoContext _dbContext;

        public EffectRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific Wirkung stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<Effect> ApplyFilter(EffectFilter filter, IMongoCollection<Effect> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<Effect> bsonFilter = Builders<Effect>.Filter.Empty;
            List<FilterDefinition<Effect>> bsonFilterList = new List<FilterDefinition<Effect>> { bsonFilter };

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.Name))
            {
                // Filter exact match for Name.
                bsonFilter = Builders<Effect>.Filter.Eq(nameof(Effect.Name), filter.Name.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<Effect> queryFilter = Builders<Effect>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Wirkung.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<Effect> ApplySorting(EffectFilter filter, IMongoCollection<Effect> collection)
        {
            SortDefinitionBuilder<Effect> sortDefinitionBuilder = Builders<Effect>.Sort;
            List<SortDefinition<Effect>> sortDefinition = new List<SortDefinition<Effect>>();

            // Add appropriate sort key and sort direction according to each sort value.
            foreach (KeyValuePair<string, string> sortValue in filter.SortValues)
            {
                if (sortValue.Value == Constants.Descending)
                {
                    // Sort by descending if explicitly specified.
                    sortDefinition.Add(sortDefinitionBuilder.Descending(sortValue.Key));
                }
                else
                {
                    // Sort by ascending if explicitly specified.
                    sortDefinition.Add(sortDefinitionBuilder.Ascending(sortValue.Key));
                }
            }

            return sortDefinitionBuilder.Combine(sortDefinition);
        }
    }
}