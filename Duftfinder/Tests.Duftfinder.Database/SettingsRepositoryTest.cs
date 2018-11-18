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
    public class SettingsRepositoryTest
    {
        private ISettingsRepository _settingsRepository;

        // Gets test database from App.config of testing project.
        private MongoContext _dbContext;

        private FindOptions _findOptions;

       [SetUp]
        public void SetUp()
        {
            _dbContext = new MongoContext();
            _settingsRepository = new SettingsRepository(_dbContext);

            _findOptions = new FindOptions { Collation = new Collation(Constants.de, strength: CollationStrength.Primary) };
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(Substance));
            _dbContext.Database.DropCollection(nameof(Molecule));
            _dbContext.Database.DropCollection(nameof(Category));
        }

        [Test]
        public async Task InitDatabase_GivenMongoDataInitialized_ShouldReturn_SubstancesCount14_FromDatabase()
        {
            // Arrange
            IMongoCollection<Substance> collection = _dbContext.Database.GetCollection<Substance>(typeof(Substance).Name);

            // Act
            await _settingsRepository.InitializeSubstancesAndCategoriesAsync();
            List<Substance> result = await collection.Find(new BsonDocument(), _findOptions).ToListAsync();

            // Assert
            Assert.AreEqual(14, result.Count);
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticAlcohol"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticAldehyde"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticEster"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Cumarin"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Ester"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Ketone"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Monoterpene"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Monoterpenole"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Phenole"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Aldehyde"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Oxide"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Phenylpropanderivate"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Sesquiterpene"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "SesquiterpenoleDiterpenole"));
        }

        [Test]
        public async Task InitDatabase_GivenMongoDataInitialized_ShouldReturn_MoleculesCount14_FromDatabase()
        {
            // Arrange
            IMongoCollection<Molecule> collection = _dbContext.Database.GetCollection<Molecule>(typeof(Molecule).Name);

            // Act
            await _settingsRepository.InitializeSubstancesAndCategoriesAsync();
            List<Molecule> result = await collection.Find(new BsonDocument(), _findOptions).ToListAsync();

            // Assert
            Assert.AreEqual(14, result.Count);
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticAlcohol"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticAldehyde"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "AromaticEster"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Cumarin"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Ester"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Ketone"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Monoterpene"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Monoterpenole"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Phenole"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Aldehyde"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Oxide"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Phenylpropanderivate"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "Sesquiterpene"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "SesquiterpenoleDiterpenole"));
        }

        [Test]
        public async Task InitDatabase_GivenMongoDataInitialized_ShouldReturn_CategoriesCount19_FromDatabase()
        {
            // Arrange
            IMongoCollection<Category> collection = _dbContext.Database.GetCollection<Category>(typeof(Category).Name);

            // Act
            await _settingsRepository.InitializeSubstancesAndCategoriesAsync();
            List<Category> result = await collection.Find(new BsonDocument(), _findOptions).ToListAsync();

            // Assert
            Assert.AreEqual(19, result.Count);
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "Pain"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "Muscle"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "SkinAndScars"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "Tissue"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "BloodCirculation"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "NervousSystemGeneral"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "NervousSystemNeurotransmitter"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "EntericNervousSystem"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "EndocrineHormones"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "ImmuneSystem"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "Inflammation"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "IntegrationYoungerSelf"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "OrderInCircle"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "AccessToInnerStrength"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "BlockageDissolving"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "AchievingGoalsAttention"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "AchievingGoalsActivationLevel"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "AchievingGoalsNerves"));
            Assert.IsNotNull(result.SingleOrDefault(c => c.Name == "MeridiansAttackYinYang"));
        }
    }
}