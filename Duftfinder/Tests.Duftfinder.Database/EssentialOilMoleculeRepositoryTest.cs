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

namespace Tests.Duftfinder.Database
{
    [TestFixture]
    public class EssentialOilMoleculeRepositoryTest
    {
        private IEssentialOilMoleculeRepository _essentialOilMoleculeRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _essentialOilMoleculeRepository = new EssentialOilMoleculeRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(EssentialOilMolecule));
        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOilMolecules_ShouldReturn_AllEssentialOilMoleculesUnsorted_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<EssentialOilMolecule> result = await _essentialOilMoleculeRepository.GetAllAsync(new EssentialOilMoleculeFilter());

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("5a339b56f36d281276999f49", result[0].Id);
            Assert.AreEqual("5a339b56f36d281276999f47", result[1].Id);
            Assert.AreEqual("5a339b56f36d281276999f48", result[2].Id);
        }

        [Test]
        public async Task GetByFilterAsync_GivenEssentialOilIdFilter_ShouldGet_EssentialOilMoleculeThatEqualsEssentialOilId_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            EssentialOilMoleculeFilter filter = new EssentialOilMoleculeFilter { EssentialOilId = "5a339b56f36d281276999f45" };

            // Act
            IList<EssentialOilMolecule> result = await _essentialOilMoleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Id == "5a339b56f36d281276999f49"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenMoleculeIdFilter_ShouldGet_EssentialOilMoleculeThatEqualsMoleculeId_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            EssentialOilMoleculeFilter filter = new EssentialOilMoleculeFilter { MoleculeId = "5a339b56f36d281276999f43" };

            // Act
            IList<EssentialOilMolecule> result = await _essentialOilMoleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsNotNull(result[0].Id == "5a339b56f36d281276999f49");
            Assert.IsNotNull(result[0].Id == "5a339b56f36d281276999f47");
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _essentialOilMoleculeRepository.InsertAsync(new EssentialOilMolecule { Id = "5a339b56f36d281276999f49", EssentialOilId = "5a339b56f36d281276999f45", MoleculeId = "5a339b56f36d281276999f43" });
            await _essentialOilMoleculeRepository.InsertAsync(new EssentialOilMolecule { Id = "5a339b56f36d281276999f47", EssentialOilId = "5a339b56f36d281276999f42", MoleculeId = "5a339b56f36d281276999f43" });
            await _essentialOilMoleculeRepository.InsertAsync(new EssentialOilMolecule { Id = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f44", MoleculeId = "5a339b56f36d281276999f46" });

        }
    }
}