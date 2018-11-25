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
    /// Represents the store objects of User.
    /// The basic functionality is implemented in Repository.cs.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class UserRepository : Repository<User, UserFilter>, IUserRepository
    {
        private readonly MongoContext _dbContext;

        public UserRepository(MongoContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Is used to filter for specific User stuff.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override FilterDefinition<User> ApplyFilter(UserFilter filter, IMongoCollection<User> collection)
        {
            // Create list of filters and pass empty filter to list, should there be no custom filter.
            FilterDefinition<User> bsonFilter = Builders<User>.Filter.Empty;
            List<FilterDefinition<User>> bsonFilterList = new List<FilterDefinition<User>>{ bsonFilter};

            // Apply custom filters.
            if (!string.IsNullOrEmpty(filter.Email))
            {
                // Filter exact match for Email.
                bsonFilter = Builders<User>.Filter.Eq(nameof(User.Email), filter.Email.Trim());
                bsonFilterList.Add(bsonFilter);
            }

            // Chain all filters.
            FilterDefinition<User> queryFilter = Builders<User>.Filter.And(bsonFilterList);

            return queryFilter;
        }

        /// <summary>
        /// Is used to sort the Users.
        /// Sorting is done by the defined sort direction & sort key of the filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SortDefinition<User> ApplySorting(UserFilter filter, IMongoCollection<User> collection)
        {
            SortDefinitionBuilder<User> sortDefinitionBuilder = Builders<User>.Sort;
            List<SortDefinition<User>> sortDefinition = new List<SortDefinition<User>>();

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