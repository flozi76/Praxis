using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    ///  Represents the interface for the business logic service of a specific type.
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IService<TEntity, TFilter> where TEntity : Entity where TFilter : Filter
    {
        Task<IList<TEntity>> GetByFilterAsync(TFilter filter);

        Task<IList<TEntity>> GetAllAsync(TFilter filter);

        Task<TEntity> GetByIdAsync(string id);

        Task<ValidationResultList> InsertAsync(TEntity entity);

        Task<ValidationResultList> UpdateAsync(TEntity entity);

        Task<ValidationResultList> DeleteAsync(string id);
    }
}
