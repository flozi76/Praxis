﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of "Stoffklasse".
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class SubstanceRepository : Repository<Substance, SubstanceFilter>, ISubstanceRepository
    {
        private readonly MongoContext _dbContext;

        public SubstanceRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific Stoffklasse stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<Substance> ApplyFilter(SubstanceFilter filter, IMongoCollection<Substance> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<Substance> bsonFilter = Builders<Substance>.Filter.Empty;
            List<FilterDefinition<Substance>> bsonFilterList = new List<FilterDefinition<Substance>> { bsonFilter };

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.Name))
            {
                // Filter exact match for Name.
                bsonFilter = Builders<Substance>.Filter.Eq(nameof(Substance.Name), filter.Name.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<Substance> queryFilter = Builders<Substance>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Stoffklassen.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<Substance> ApplySorting(SubstanceFilter filter, IMongoCollection<Substance> collection)
        {
            SortDefinitionBuilder<Substance> sortDefinitionBuilder = Builders<Substance>.Sort;
            List<SortDefinition<Substance>> sortDefinition = new List<SortDefinition<Substance>>();

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
