using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Represents the interface for the generic store of objects of a specific type.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    public interface IRepository<TEntity, TFilter> where TEntity : Entity where TFilter : Filter
    {
        Task<IList<TEntity>> GetByFilterAsync(TFilter filter);

        Task<TEntity> GetByIdAsync(string id);

        Task<IList<TEntity>> GetAllAsync(TFilter filter);

        Task<ValidationResultList> InsertAsync(TEntity entity);

        Task<ValidationResultList> UpdateAsync(TEntity entity);

        Task<ValidationResultList> DeleteAsync(string id);
    }
}
