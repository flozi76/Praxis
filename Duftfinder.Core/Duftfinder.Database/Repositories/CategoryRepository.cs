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
	///     Represents the store objects of "Wirkungskategorie".
	///     The basic functionality is implemented in Repository.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class CategoryRepository : Repository<Category, CategoryFilter>, ICategoryRepository
	{
		private readonly MongoContext _dbContext;

		public CategoryRepository(MongoContext context) : base(context)
		{
			_dbContext = context;
		}

		/// <summary>
		///     Is used to filter for specific Wirkungskategorie stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<Category> ApplyFilter(CategoryFilter filter,
			IMongoCollection<Category> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<Category>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<Category>> {bsonFilter};

			// Apply custom filters.
			if (!string.IsNullOrEmpty(filter.Name))
			{
				// Filter exact match for Name.
				bsonFilter = Builders<Category>.Filter.Eq(nameof(Category.Name), filter.Name.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<Category>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Wirkungskategorie.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<Category> ApplySorting(CategoryFilter filter,
			IMongoCollection<Category> collection)
		{
			var sortDefinitionBuilder = Builders<Category>.Sort;
			var sortDefinition = new List<SortDefinition<Category>>();

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