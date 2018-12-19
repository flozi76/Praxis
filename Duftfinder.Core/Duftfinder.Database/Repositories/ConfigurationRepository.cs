using System.Collections.Generic;
using Duftfinder.Database.Mongo;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
	/// <summary>
	///     Represents the store objects of Configuration.
	///     The basic functionality is implemented in Repository.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class ConfigurationRepository : Repository<Configuration, ConfigurationFilter>, IConfigurationRepository
	{
		private readonly MongoContext _dbContext;

		public ConfigurationRepository(MongoContext context) : base(context)
		{
			_dbContext = context;
		}

		/// <summary>
		///     Is used to filter for specific Configuration stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<Configuration> ApplyFilter(ConfigurationFilter filter,
			IMongoCollection<Configuration> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<Configuration>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<Configuration>> {bsonFilter};

			// Apply custom filters.
			if (!string.IsNullOrEmpty(filter.Key))
			{
				// Filter exact match for Key.
				bsonFilter = Builders<Configuration>.Filter.Eq(nameof(Configuration.Key), filter.Key.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<Configuration>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Configurations.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<Configuration> ApplySorting(ConfigurationFilter filter,
			IMongoCollection<Configuration> collection)
		{
			var sortDefinitionBuilder = Builders<Configuration>.Sort;
			var sortDefinition = new List<SortDefinition<Configuration>>();

			// Add appropriate sort key and sort direction according to each sort value.
			foreach (var sortValue in filter.SortValues)
				if (sortValue.Value == Constants.Descending)
					sortDefinition.Add(sortDefinitionBuilder.Descending(sortValue.Key));
				else
					sortDefinition.Add(sortDefinitionBuilder.Ascending(sortValue.Key));

			return sortDefinitionBuilder.Combine(sortDefinition);
		}
	}
}