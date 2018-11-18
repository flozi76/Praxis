using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Duftfinder.Domain.Helpers;
using log4net;


namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of "Ätherische Öle".
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilRepository : Repository<EssentialOil, EssentialOilFilter>, IEssentialOilRepository
    {
        private readonly MongoContext _dbContext;

        public EssentialOilRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific Ätherische Öle stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<EssentialOil> ApplyFilter(EssentialOilFilter filter, IMongoCollection<EssentialOil> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<EssentialOil> bsonFilter = Builders<EssentialOil>.Filter.Empty;
            List<FilterDefinition<EssentialOil>> bsonFilterList = new List<FilterDefinition<EssentialOil>>{ bsonFilter};

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.Name))
            {
                // Filter exact match for Name.
                bsonFilter = Builders<EssentialOil>.Filter.Eq(nameof(EssentialOil.Name), filter.Name.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                // Filter if essential oil names contain SearchText.
                bsonFilter = Builders<EssentialOil>.Filter.Where(e => e.Name.ToLower().Trim().Contains(filter.SearchText.ToLower().Trim()));
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<EssentialOil> queryFilter = Builders<EssentialOil>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Ätherische Öle.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<EssentialOil> ApplySorting(EssentialOilFilter filter, IMongoCollection<EssentialOil> collection)
        {
            SortDefinitionBuilder<EssentialOil> sortDefinitionBuilder = Builders<EssentialOil>.Sort;
            List<SortDefinition<EssentialOil>> sortDefinition = new List<SortDefinition<EssentialOil>>();

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