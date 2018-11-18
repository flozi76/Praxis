using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using Duftfinder.Database.Repositories;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moq;
using NUnit.Framework;
using Duftfinder.Resources;

namespace Tests.Duftfinder.Database
{
    [TestFixture]
    public class CategoryRepositoryTest
    {
        private ICategoryRepository _categoryRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(Category));
        }

        [Test]
        public async Task GetAllAsync_GivenCategories_ShouldReturn_AllCategoriesBySortOrder_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<Category> result = await _categoryRepository.GetAllAsync(new CategoryFilter());

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a category", result[0].Name);
            Assert.AreEqual("test", result[1].Name);
            Assert.AreEqual("some category", result[2].Name);
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithMatchingName_ShouldGet_CategoryThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            CategoryFilter filter = new CategoryFilter { Name = "a category" };

            // Act
            IList<Category> result = await _categoryRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Name == "a category"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithNoMatchingName_ShouldGet_NoCategory_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            CategoryFilter filter = new CategoryFilter { Name = "category" };

            // Act
            IList<Category> result = await _categoryRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _categoryRepository.InsertAsync(new Category { Name = "test", SortOrder = 2 });
            await _categoryRepository.InsertAsync(new Category { Name = "a category", SortOrder = 1 });
            await _categoryRepository.InsertAsync(new Category { Name = "some category", SortOrder = 3 });
        }
    }
}