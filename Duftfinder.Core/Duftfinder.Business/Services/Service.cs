using System.Collections.Generic;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Represents the business logic service of a specific type.
	/// </summary>
	/// <author>Anna Krebs</author>
	public abstract class Service<TEntity, TFilter, TRepository> : IService<TEntity, TFilter> where TEntity : Entity
		where TFilter : Filter
		where TRepository : IRepository<TEntity, TFilter>
	{
		protected Service(TRepository repository)
		{
			Repository = repository;
		}

		protected virtual TRepository Repository { get; }

		/// <summary>
		///     Get all values for specific entity according to filter.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <returns></returns>
		public Task<IList<TEntity>> GetByFilterAsync(TFilter filter)
		{
			return Repository.GetByFilterAsync(filter);
		}

		/// <summary>
		///     Get all values for specific entity from database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="filter"></param>
		/// <returns></returns>
		public Task<IList<TEntity>> GetAllAsync(TFilter filter)
		{
			return Repository.GetAllAsync(filter);
		}

		/// <summary>
		///     Gets one specific entity from database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="id"></param>
		public Task<TEntity> GetByIdAsync(string id)
		{
			return Repository.GetByIdAsync(id);
		}

		/// <summary>
		///     Insert specific entity in database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="entity"></param>
		public Task<ValidationResultList> InsertAsync(TEntity entity)
		{
			var validationResult = Repository.InsertAsync(entity);

			return validationResult;
		}

		/// <summary>
		///     Updates specific entity in database..
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="entity"></param>
		public Task<ValidationResultList> UpdateAsync(TEntity entity)
		{
			var validationResult = Repository.UpdateAsync(entity);

			return validationResult;
		}

		/// <summary>
		///     Deletes specific entity in database.
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="id"></param>
		public Task<ValidationResultList> DeleteAsync(string id)
		{
			var validationResult = Repository.DeleteAsync(id);

			return validationResult;
		}
	}
}