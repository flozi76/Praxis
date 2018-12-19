using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
	/// <summary>
	///     Represents the store objects of "Ätherische Öle".
	///     The basic functionality is implemented in Repository.cs.
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
		///     Is used to filter for specific Ätherische Öle stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<EssentialOil> ApplyFilter(EssentialOilFilter filter,
			IMongoCollection<EssentialOil> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<EssentialOil>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<EssentialOil>> {bsonFilter};

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
				bsonFilter = Builders<EssentialOil>.Filter.Where(e =>
					e.Name.ToLower().Trim().Contains(filter.SearchText.ToLower().Trim()));
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<EssentialOil>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Ätherische Öle.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<EssentialOil> ApplySorting(EssentialOilFilter filter,
			IMongoCollection<EssentialOil> collection)
		{
			var sortDefinitionBuilder = Builders<EssentialOil>.Sort;
			var sortDefinition = new List<SortDefinition<EssentialOil>>();

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