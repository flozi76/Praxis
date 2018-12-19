using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
	/// <summary>
	///     Contains business logic for "Wirkungskategorie".
	///     Basic functionality is done in Service.cs.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class CategoryService : Service<Category, CategoryFilter, ICategoryRepository>, ICategoryService
	{
		private readonly ICategoryRepository _categoryRepository;

		public CategoryService(ICategoryRepository categoryRepository)
			: base(categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}
	}
}