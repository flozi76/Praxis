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
    public class MoleculeRepositoryTest
    {
        private IMoleculeRepository _moleculeRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _moleculeRepository = new MoleculeRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(Molecule));
        }

        [Test]
        public async Task GetAllAsync_GivenMolecules_ShouldReturn_AllMoleculesBySortOrder_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<Molecule> result = await _moleculeRepository.GetAllAsync(new MoleculeFilter());

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("aab", result[0].Name);
            Assert.AreEqual("aac", result[1].Name);
            Assert.AreEqual("aad", result[2].Name);
            Assert.AreEqual("aaa", result[3].Name);
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithMatchingName_ShouldGet_MoleculeThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            MoleculeFilter filter = new MoleculeFilter { Name = "aaa" };

            // Act
            IList<Molecule> result = await _moleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Name == "aaa"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithNoMatchingName_ShouldGet_NoMolecule_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            MoleculeFilter filter = new MoleculeFilter { Name = "aa" };

            // Act
            IList<Molecule> result = await _moleculeRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _moleculeRepository.InsertAsync(new Molecule { Name = "aad", IsGeneral = true, SubstanceIdString = "5a339b56f36d281276999f43" });
            await _moleculeRepository.InsertAsync(new Molecule { Name = "aac", IsGeneral = false, SubstanceIdString = "5a339b56f36d281276999f42" });
            await _moleculeRepository.InsertAsync(new Molecule { Name = "aab", IsGeneral = true, SubstanceIdString = "5a339b56f36d281276999f42" });
            await _moleculeRepository.InsertAsync(new Molecule { Name = "aaa", IsGeneral = false, SubstanceIdString = "5a339b56f36d281276999f43" });
        }
    }
}