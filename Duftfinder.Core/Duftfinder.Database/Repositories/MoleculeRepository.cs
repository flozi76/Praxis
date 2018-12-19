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
	///     Represents the store objects of "Molekül".
	///     The basic functionality is implemented in Repository.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class MoleculeRepository : Repository<Molecule, MoleculeFilter>, IMoleculeRepository
	{
		private readonly MongoContext _dbContext;

		public MoleculeRepository(MongoContext context) : base(context)
		{
			_dbContext = context;
		}

		/// <summary>
		///     Is used to filter for specific Molekül stuff.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override FilterDefinition<Molecule> ApplyFilter(MoleculeFilter filter,
			IMongoCollection<Molecule> collection)
		{
			// Create list of filters and pass empty filter to list, should there be no custom filter.
			var bsonFilter = Builders<Molecule>.Filter.Empty;
			var bsonFilterList = new List<FilterDefinition<Molecule>> {bsonFilter};

			// Apply custom filters.
			if (!string.IsNullOrEmpty(filter.Name))
			{
				// Filter exact match for Name.
				bsonFilter = Builders<Molecule>.Filter.Eq(nameof(Molecule.Name), filter.Name.Trim());
				bsonFilterList.Add(bsonFilter);
			}

			// Chain all filters.
			var queryFilter = Builders<Molecule>.Filter.And(bsonFilterList);

			return queryFilter;
		}

		/// <summary>
		///     Is used to sort the Molekül.
		///     Sorting is done by the defined sort direction & sort key of the filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SortDefinition<Molecule> ApplySorting(MoleculeFilter filter,
			IMongoCollection<Molecule> collection)
		{
			var sortDefinitionBuilder = Builders<Molecule>.Sort;
			var sortDefinition = new List<SortDefinition<Molecule>>();

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