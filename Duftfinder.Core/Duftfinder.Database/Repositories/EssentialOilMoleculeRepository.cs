using System.Collections.Generic;
using Duftfinder.Database.Helpers;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
	/// <summary>
	///     Represents the store objects of "Moleküle" to "Ätherisches Öl".
	///     The basic functionality is implemented in Repository.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class EssentialOilMoleculeRepository : Repository<EssentialOilMolecule, EssentialOilMoleculeFilter>,
		IEssentialOilMoleculeRepository
	{
		private readonly MongoContext _dbContext;

		public EssentialOilMoleculeRepository(MongoContext context) : base(context)
		{
			_dbContext = context;
		}

		/// <summary>
		///     Is used to filter for specific Moleküle for Ätherisches Öl stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<EssentialOilMolecule> ApplyFilter(EssentialOilMoleculeFilter filter,
			IMongoCollection<EssentialOilMolecule> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<EssentialOilMolecule>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<EssentialOilMolecule>> {bsonFilter};

			// Apply custom filters.
			if (!string.IsNullOrEmpty(filter.EssentialOilId))
			{
				// Filter exact match for EssentialOilId.
				bsonFilter = Builders<EssentialOilMolecule>.Filter.Eq(nameof(EssentialOilMolecule.EssentialOilId),
					filter.EssentialOilId.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			if (!string.IsNullOrEmpty(filter.MoleculeId))
			{
				// Filter exact match for MoleculeId.
				bsonFilter = Builders<EssentialOilMolecule>.Filter.Eq(nameof(EssentialOilMolecule.MoleculeId),
					filter.MoleculeId.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<EssentialOilMolecule>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Moleküle for Ätherisches Öl.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<EssentialOilMolecule> ApplySorting(EssentialOilMoleculeFilter filter,
			IMongoCollection<EssentialOilMolecule> collection)
		{
			var sortDefinitionBuilder = Builders<EssentialOilMolecule>.Sort;
			var sortDefinition = new List<SortDefinition<EssentialOilMolecule>>();

			return sortDefinitionBuilder.Combine(sortDefinition);
		}
	}
}