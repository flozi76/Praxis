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
    public class EssentialOilRepositoryTest
    {
        private IEssentialOilRepository _essentialOilRepository;

        // Gets test database from App.config of testing project.
        private readonly MongoContext _dbContext = new MongoContext();

        [SetUp]
        public void SetUp()
        {
            _essentialOilRepository = new EssentialOilRepository(_dbContext);

            // Delete collection in MongoDB before every test.
            _dbContext.Database.DropCollection(nameof(EssentialOil));
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOils_ShouldReturn_AllEssentialOilsAscending_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("another oil", result[0].Name);
            Assert.AreEqual("new", result[1].Name);
            Assert.AreEqual("NewEssentialOil", result[2].Name);
            Assert.AreEqual("NewOil", result[3].Name);
            Assert.AreEqual("Oil", result[4].Name);
            Assert.AreEqual("XyzOil", result[5].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOilsWithSortOrderDescending_ShouldReturn_AllEssentialOilsDescending_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { SortValues = new Dictionary<string, string> { { "Name", Constants.Descending } }
            };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("another oil", result[5].Name);
            Assert.AreEqual("new", result[4].Name);
            Assert.AreEqual("NewEssentialOil", result[3].Name);
            Assert.AreEqual("NewOil", result[2].Name);
            Assert.AreEqual("Oil", result[1].Name);
            Assert.AreEqual("XyzOil", result[0].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOilsWithSortKeyDescription_ShouldReturn_AllEssentialOilsSortedByDescription_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { SortValues = new Dictionary<string, string> { { "Description", Constants.Ascending} } };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("Oil", result[0].Name);
            Assert.AreEqual("XyzOil", result[1].Name);
            Assert.AreEqual("NewOil", result[2].Name);
            Assert.AreEqual("NewEssentialOil", result[3].Name);
            Assert.AreEqual("new", result[4].Name);
            Assert.AreEqual("another oil", result[5].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOilsWithSortKeyDescriptionAndSortOrderDescending_ShouldReturn_AllEssentialOilsSortedByDescriptionDescending_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { SortValues = new Dictionary<string, string> { { "Description", Constants.Descending} } };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("XyzOil", result[5].Name);
            Assert.AreEqual("Oil", result[4].Name);
            Assert.AreEqual("NewOil", result[3].Name);
            Assert.AreEqual("new", result[2].Name);
            Assert.AreEqual("NewEssentialOil", result[1].Name);
            Assert.AreEqual("another oil", result[0].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEssentialOilsWithFilterName_ShouldReturn_AllEssentialOilsWithName_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { Name = "new"};

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("new", result[0].Name);
        }

        [Test]
        public async Task GetByIdAsync_GivenEssentialOil_ShouldGet_EssentialOil_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { Name = "new" };
            IList<EssentialOil> essentialOils = await _essentialOilRepository.GetByFilterAsync(filter);
            EssentialOil essentialOil = essentialOils.SingleOrDefault();

            // Act
             EssentialOil result = await _essentialOilRepository.GetByIdAsync(essentialOil.Id);

            // Assert
            Assert.AreEqual("new", result.Name);
            Assert.AreEqual(essentialOil.Id, result.Id);
        }

        [Test]
        public async Task GetByIdAsync_GivenNoMatchingId_Throws_ArgumentNullException()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            ObjectId id = ObjectId.GenerateNewId();

            // Act
            Assert.That(() => _essentialOilRepository.GetByIdAsync(id.ToString()), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task InsertAsync_GivenNewEssentialOil_ShouldInsert_NewEssentialOil_InDatabase()
        {
            // Arrange
            EssentialOil essentialOil = new EssentialOil {Name = "NewEssentialOil" };
            EssentialOilFilter filter = new EssentialOilFilter { Name = essentialOil.Name.Trim() };
            IList<EssentialOil> essentialOilsBeforInsert = await _essentialOilRepository.GetByFilterAsync(filter);

            // Act
            ValidationResultList validationResult = await _essentialOilRepository.InsertAsync(essentialOil);
            
            // Assert
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);

            Assert.AreEqual(0, essentialOilsBeforInsert.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("NewEssentialOil", result[0].Name);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task InsertAsync_GivenEssentialOilAlreadyExists_ShouldNotInsert_EssentialOil_InDatabase()
        {
            // Arrange
            EssentialOil essentialOil = new EssentialOil {Name = "NewEssentialOil" };
            EssentialOilFilter filter = new EssentialOilFilter { Name = essentialOil.Name.Trim() };

            // Insert essential oil in database.
            await _essentialOilRepository.InsertAsync(essentialOil);
            IList<EssentialOil> essentialOilsBeforeInsert = await _essentialOilRepository.GetByFilterAsync(filter);

            // Act
            ValidationResultList validationResult = await _essentialOilRepository.InsertAsync(essentialOil);
            
            // Assert
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;

            Assert.AreEqual(1, essentialOilsBeforeInsert.Count);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(Resources.Error_EntityAlreadyExists, validationResultErrorMessage);
        }

        [Test]
        public async Task UpdateAsync_GivenNewEssentialOil_ShouldUpdate_NewEssentialOil_To_UpdatedEssentialOil_InDatabase()
        {
            // Arrange
            EssentialOil essentialOil = new EssentialOil { Name = "NewEssentialOil" };

            // Insert essential oil in database.
            await _essentialOilRepository.InsertAsync(essentialOil);
            IList<EssentialOil> essentialOilsBeforeUpdate = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            essentialOilsBeforeUpdate[0].Name = "UpdatedEssentialOil";

            // Act
            ValidationResultList validationResult = await _essentialOilRepository.UpdateAsync(essentialOilsBeforeUpdate[0]);

            // Assert
            IList<EssentialOil> essentialOils = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            Assert.AreEqual(1, essentialOilsBeforeUpdate.Count);
            Assert.AreEqual(1, essentialOils.Count);
            Assert.AreEqual("UpdatedEssentialOil", essentialOils[0].Name);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task UpdateAsync_GivenEssentialOilAlreadyExists_ShouldUpdate_NewEssentialOil_To_newessentialoil_InDatabase()
        {
            // Arrange
            EssentialOil essentialOil = new EssentialOil { Name = "NewEssentialOil" };
            
            // Insert essential oil in database.
            await _essentialOilRepository.InsertAsync(essentialOil);
            IList<EssentialOil> essentialOilsBeforeUpdate = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            essentialOilsBeforeUpdate[0].Name = "newessentialoil";
            essentialOilsBeforeUpdate[0].Description = "NewDescription";

            // Act
            ValidationResultList validationResult = await _essentialOilRepository.UpdateAsync(essentialOilsBeforeUpdate[0]);

            // Assert
            IList<EssentialOil> result = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            Assert.AreEqual(1, essentialOilsBeforeUpdate.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("newessentialoil", result[0].Name);
            Assert.AreEqual("NewDescription", result[0].Description);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task UpdateAsync_GivenNoMatchingId_ShouldNotUpdate_EssentialOil_InDatabase()
        {
            // Arrange
            EssentialOil essentialOil = new EssentialOil { ObjectId = ObjectId.GenerateNewId(), Name = "NewEssentialOil" };

            // Act 
            ValidationResultList validationResult = await _essentialOilRepository.UpdateAsync(essentialOil);

            // Assert
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;

            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(string.Format(Resources.Error_NoEntityWithIdFound, essentialOil.Id), validationResultErrorMessage);
        }

        [Test]
        public async Task DeleteAsync_GivenEssentialOilExists_Deletes_EssentialOilFromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { Name = "NewEssentialOil" };
            IList<EssentialOil> essentialOilBeforeDelete = await _essentialOilRepository.GetByFilterAsync(filter);
            EssentialOil essentialOil = essentialOilBeforeDelete.SingleOrDefault();

            // Act 
            ValidationResultList validationResult = await _essentialOilRepository.DeleteAsync(essentialOil.Id);

            // Assert
            IList<EssentialOil> essentialOils = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());
            IList<EssentialOil> essentialOilAfterDelete = await _essentialOilRepository.GetByFilterAsync(filter);

            Assert.AreEqual(1, essentialOilBeforeDelete.Count);
            Assert.AreEqual(0, essentialOilAfterDelete.Count);
            Assert.AreEqual(5, essentialOils.Count);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteAsync_GivenNoMatchingId_ShouldNotDelete_EssentialOil_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            IList<EssentialOil> essentialOilsBeforeDelete = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            ObjectId id = ObjectId.GenerateNewId();

            // Act 
            ValidationResultList validationResult = await _essentialOilRepository.DeleteAsync(id.ToString());

            // Assert
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;
            IList<EssentialOil> essentialOilsAfterDelete = await _essentialOilRepository.GetAllAsync(new EssentialOilFilter());

            Assert.AreEqual(essentialOilsBeforeDelete.Count, essentialOilsAfterDelete.Count);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(string.Format(Resources.Error_NoEntityWithIdDeleted, id), validationResultErrorMessage);
        }

        [Test]
        public void DeleteAsync_GivenIdIsNull_Throws_ArgumentNullException()
        {
            // Arrange
            string id = null;

            // Assert
            Assert.That(() => _essentialOilRepository.DeleteAsync(id), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithMatchingName_ShouldGet_EssentialOilThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { Name = "New" };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "new"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithNoMatchingName_ShouldGet_NoEssentialOil_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { Name = "ew" };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetByFilterAsync_GivenSearchTextFilter_ShouldGet_EssentialOilThatContainSearchText_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { SearchText = "ew" };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "NewEssentialOil"));
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "new"));
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "NewOil"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenSearchTextFilterCaseInsenitive_ShouldGet_EssentialOilThatContainSearchText_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEssentialOils();
            EssentialOilFilter filter = new EssentialOilFilter { SearchText = "New" };

            // Act
            IList<EssentialOil> result = await _essentialOilRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "NewEssentialOil"));
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "new"));
            Assert.IsNotNull(result.FirstOrDefault(e => e.Name == "NewOil"));
        }

        private async Task PrepareDatabaseWithEssentialOils()
        {
            // Inserts Essential Oils in DB for testing.
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "NewEssentialOil", Description = "newdescription" });
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "new", Description = "NewDescription" });
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "Oil"});
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "NewOil", Description = "a description"});
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "XyzOil"});
            await _essentialOilRepository.InsertAsync(new EssentialOil { Name = "another oil", Description = "Some Descritpion"});
        }
    }
}