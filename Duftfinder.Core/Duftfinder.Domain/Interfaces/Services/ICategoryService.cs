using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service of "Wirkungskategorie".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface ICategoryService : IService<Category, CategoryFilter>
	{
	}
}