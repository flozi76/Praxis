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
    public class SubstanceRepositoryTest
    {
        private ISubstanceRepository _substanceRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _substanceRepository = new SubstanceRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(Substance));
        }
        
        [Test]
        public async Task GetAllAsync_GivenSubstances_ShouldReturn_AllSubstancesBySortOrder_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<Substance> result = await _substanceRepository.GetAllAsync(new SubstanceFilter());

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a substance", result[0].Name);
            Assert.AreEqual("test", result[1].Name);
            Assert.AreEqual("some substance", result[2].Name);
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithMatchingName_ShouldGet_SubstanceThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            SubstanceFilter filter = new SubstanceFilter { Name = "a substance" };

            // Act
            IList<Substance> result = await _substanceRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Name == "a substance"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithNoMatchingName_ShouldGet_NoSubstance_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            SubstanceFilter filter = new SubstanceFilter { Name = "substance" };

            // Act
            IList<Substance> result = await _substanceRepository.GetByFilterAsync(filter);
            
            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _substanceRepository.InsertAsync(new Substance { Name = "test", SortOrder = 2 });
            await _substanceRepository.InsertAsync(new Substance { Name = "a substance", SortOrder = 1 });
            await _substanceRepository.InsertAsync(new Substance { Name = "some substance", SortOrder = 3 });
        }
    }
}