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
    public class EffectMoleculeRepositoryTest
    {
        private IEffectMoleculeRepository _effectMoleculeRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _effectMoleculeRepository = new EffectMoleculeRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(EffectMolecule));
        }

        [Test]
        public async Task GetAllAsync_GivenEffectMolecules_ShouldReturn_AllEffectMoleculesUnsorted_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<EffectMolecule> result = await _effectMoleculeRepository.GetAllAsync(new EffectMoleculeFilter());

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("5a339b56f36d281276999f49", result[0].Id);
            Assert.AreEqual("5a339b56f36d281276999f47", result[1].Id);
            Assert.AreEqual("5a339b56f36d281276999f48", result[2].Id);
        }

        [Test]
        public async Task GetByFilterAsync_GivenEffectIdFilter_ShouldGet_EffectMoleculeThatEqualsEffectId_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            EffectMoleculeFilter filter = new EffectMoleculeFilter { EffectId = "5a339b56f36d281276999f45" };

            // Act
            IList<EffectMolecule> result = await _effectMoleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Id == "5a339b56f36d281276999f49"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenMoleculeIdFilter_ShouldGet_EffectMoleculeThatEqualsMoleculeId_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            EffectMoleculeFilter filter = new EffectMoleculeFilter { MoleculeId = "5a339b56f36d281276999f43" };

            // Act
            IList<EffectMolecule> result = await _effectMoleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsNotNull(result[0].Id == "5a339b56f36d281276999f49");
            Assert.IsNotNull(result[0].Id == "5a339b56f36d281276999f47");
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _effectMoleculeRepository.InsertAsync(new EffectMolecule { Id = "5a339b56f36d281276999f49", EffectId = "5a339b56f36d281276999f45", MoleculeId = "5a339b56f36d281276999f43" });
            await _effectMoleculeRepository.InsertAsync(new EffectMolecule { Id = "5a339b56f36d281276999f47", EffectId = "5a339b56f36d281276999f42", MoleculeId = "5a339b56f36d281276999f43" });
            await _effectMoleculeRepository.InsertAsync(new EffectMolecule { Id = "5a339b56f36d281276999f48", EffectId = "5a339b56f36d281276999f44", MoleculeId = "5a339b56f36d281276999f46" });

        }
    }
}