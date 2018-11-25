﻿using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using Duftfinder.Domain.Helpers;


namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the store objects of Role.
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class RoleRepository : Repository<Role, RoleFilter>, IRoleRepository
    {
        private readonly MongoContext _dbContext;

        public RoleRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific Role stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<Role> ApplyFilter(RoleFilter filter, IMongoCollection<Role> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<Role> bsonFilter = Builders<Role>.Filter.Empty;
            List<FilterDefinition<Role>> bsonFilterList = new List<FilterDefinition<Role>>{ bsonFilter};

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.Name))
            {
                // Filter exact match for Name.
                bsonFilter = Builders<Role>.Filter.Eq(nameof(Role.Name), filter.Name.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<Role> queryFilter = Builders<Role>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Roles.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<Role> ApplySorting(RoleFilter filter, IMongoCollection<Role> collection)
        {
            SortDefinitionBuilder<Role> sortDefinitionBuilder = Builders<Role>.Sort;
            List<SortDefinition<Role>> sortDefinition = new List<SortDefinition<Role>>();

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