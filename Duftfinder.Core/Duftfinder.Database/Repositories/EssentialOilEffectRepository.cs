using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
	/// <summary>
	///     Represents the store objects of "Wirkungen" to "Ätherisches Öl".
	///     The basic functionality is implemented in Repository.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilEffectRepository : Repository<EssentialOilEffect, EssentialOilEffectFilter>,
		IEssentialOilEffectRepository
	{
		private readonly MongoContext _dbContext;

		public EssentialOilEffectRepository(MongoContext context) : base(context)
		{
			_dbContext = context;
		}

		/// <summary>
		///     Is used to filter for specific Wirkungen for Ätherisches Öl stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<EssentialOilEffect> ApplyFilter(EssentialOilEffectFilter filter,
			IMongoCollection<EssentialOilEffect> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<EssentialOilEffect>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<EssentialOilEffect>> {bsonFilter};

			// Apply custom filters.
			if (!string.IsNullOrEmpty(filter.EssentialOilId))
			{
				// Filter exact match for EssentialOilId.
				bsonFilter = Builders<EssentialOilEffect>.Filter.Eq(nameof(EssentialOilEffect.EssentialOilId),
					filter.EssentialOilId.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			if (!string.IsNullOrEmpty(filter.EffectId))
			{
				// Filter exact match for EssentialOilId.
				bsonFilter =
					Builders<EssentialOilEffect>.Filter.Eq(nameof(EssentialOilEffect.EffectId), filter.EffectId.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<EssentialOilEffect>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Wirkungen for Ätherisches Öl.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<EssentialOilEffect> ApplySorting(EssentialOilEffectFilter filter,
			IMongoCollection<EssentialOilEffect> collection)
		{
			var sortDefinitionBuilder = Builders<EssentialOilEffect>.Sort;
			var sortDefinition = new List<SortDefinition<EssentialOilEffect>>();

			return sortDefinitionBuilder.Combine(sortDefinition);
		}
	}
}