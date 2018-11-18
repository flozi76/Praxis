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
    public class EffectRepositoryTest
    {
        private IEffectRepository _effectRepository;

        // Gets test database from App.config of testing project.
        private readonly MongoContext _dbContext = new MongoContext();

        [SetUp]
        public void SetUp()
        {
            _effectRepository = new EffectRepository(_dbContext);

            // Delete collection in MongoDB before every test.
            _dbContext.Database.DropCollection(nameof(Effect));
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task GetAllAsync_GivenEffects_ShouldReturn_AllEffectsAscending_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();

            // Act
            IList<Effect> result = await _effectRepository.GetAllAsync(new EffectFilter());

            // Assert
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual("another effect", result[0].Name);
            Assert.AreEqual("anothereffect", result[1].Name);
            Assert.AreEqual("Effect", result[2].Name);
            Assert.AreEqual("effects", result[3].Name);
            Assert.AreEqual("new", result[4].Name);
            Assert.AreEqual("NewEffect", result[5].Name);
            Assert.AreEqual("XyzEffect", result[6].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEffectsWithSortOrderDescending_ShouldReturn_AllEffectsDescending_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            EffectFilter filter = new EffectFilter { SortValues = new Dictionary<string, string> { { "Name", Constants.Descending } } };

            // Act
            IList<Effect> result = await _effectRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual("another effect", result[6].Name);
            Assert.AreEqual("anothereffect", result[5].Name);
            Assert.AreEqual("Effect", result[4].Name);
            Assert.AreEqual("effects", result[3].Name);
            Assert.AreEqual("new", result[2].Name);
            Assert.AreEqual("NewEffect", result[1].Name);
            Assert.AreEqual("XyzEffect", result[0].Name);
        }

        [Test]
        public async Task GetAllAsync_GivenEffectsWithFilterName_ShouldReturn_AllEffectsWithName_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            EffectFilter filter = new EffectFilter { Name = "anothereffect" };

            // Act
            IList<Effect> result = await _effectRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("anothereffect", result[0].Name);
        }

        [Test]
        public async Task GetByIdAsync_GivenEffect_ShouldGet_Effect_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            EffectFilter filter = new EffectFilter { Name = "new" };
            IList<Effect> effects = await _effectRepository.GetByFilterAsync(filter);
            Effect effect = effects.SingleOrDefault();

            // Act
            Effect result = await _effectRepository.GetByIdAsync(effect.Id);

            // Assert
            Assert.AreEqual("new", result.Name);
            Assert.AreEqual(effect.Id, result.Id);
        }

        [Test]
        public async Task GetByIdAsync_GivenNoMatchingId_Throws_ArgumentNullException()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            ObjectId id = ObjectId.GenerateNewId();

            // Act
            Assert.That(() => _effectRepository.GetByIdAsync(id.ToString()), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task InsertAsync_GivenNewEffect_ShouldInsert_NewEffect_InDatabase()
        {
            // Arrange
            Effect effect = new Effect { Name = "NewEffect" };
            EffectFilter filter = new EffectFilter { Name = effect.Name.Trim() };
            IList<Effect> effectsBeforInsert = await _effectRepository.GetByFilterAsync(filter);

            // Act
            ValidationResultList validationResult = await _effectRepository.InsertAsync(effect);

            // Assert
            IList<Effect> result = await _effectRepository.GetByFilterAsync(filter);

            Assert.AreEqual(0, effectsBeforInsert.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("NewEffect", result[0].Name);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task InsertAsync_GivenEffectAlreadyExists_ShouldNotInsert_Effect_InDatabase()
        {
            // Arrange
            Effect effect = new Effect { Name = "NewEffect" };
            EffectFilter filter = new EffectFilter { Name = effect.Name.Trim() };

            // Insert effect in database.
            await _effectRepository.InsertAsync(effect);
            IList<Effect> effectsBeforeInsert = await _effectRepository.GetByFilterAsync(filter);

            // Act
            ValidationResultList validationResult = await _effectRepository.InsertAsync(effect);

            // Assert
            IList<Effect> result = await _effectRepository.GetByFilterAsync(filter);
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;

            Assert.AreEqual(1, effectsBeforeInsert.Count);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(Resources.Error_EntityAlreadyExists, validationResultErrorMessage);
        }

        [Test]
        public async Task UpdateAsync_GivenNewEffect_ShouldUpdate_NewEffect_To_UpdatedEffect_InDatabase()
        {
            // Arrange
            Effect effect = new Effect { Name = "NewEffect" };

            // Insert effect in database.
            await _effectRepository.InsertAsync(effect);
            IList<Effect> effectsBeforeUpdate = await _effectRepository.GetAllAsync(new EffectFilter());

            effectsBeforeUpdate[0].Name = "UpdatedEffect";

            // Act
            ValidationResultList validationResult = await _effectRepository.UpdateAsync(effectsBeforeUpdate[0]);

            // Assert
            IList<Effect> effects = await _effectRepository.GetAllAsync(new EffectFilter());

            Assert.AreEqual(1, effectsBeforeUpdate.Count);
            Assert.AreEqual(1, effects.Count);
            Assert.AreEqual("UpdatedEffect", effects[0].Name);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task UpdateAsync_GivenEffectAlreadyExists_ShouldUpdate_NewEffect_To_neweffect_InDatabase()
        {
            // Arrange
            Effect effect = new Effect { Name = "NewEffect" };

            // Insert effect in database.
            await _effectRepository.InsertAsync(effect);
            IList<Effect> effectsBeforeUpdate = await _effectRepository.GetAllAsync(new EffectFilter());

            effectsBeforeUpdate[0].Name = "neweffect";
            effectsBeforeUpdate[0].Details = "NewDetails";

            // Act
            ValidationResultList validationResult = await _effectRepository.UpdateAsync(effectsBeforeUpdate[0]);

            // Assert
            IList<Effect> result = await _effectRepository.GetAllAsync(new EffectFilter());

            Assert.AreEqual(1, effectsBeforeUpdate.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("neweffect", result[0].Name);
            Assert.AreEqual("NewDetails", result[0].Details);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task UpdateAsync_GivenNoMatchingId_ShouldNotUpdate_Effect_InDatabase()
        {
            // Arrange
            Effect effect = new Effect { ObjectId = ObjectId.GenerateNewId(), Name = "NewEffect" };

            // Act 
            ValidationResultList validationResult = await _effectRepository.UpdateAsync(effect);

            // Assert
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;

            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(string.Format(Resources.Error_NoEntityWithIdFound, effect.Id), validationResultErrorMessage);
        }

        [Test]
        public async Task DeleteAsync_GivenEffectExists_Deletes_EffectFromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            EffectFilter filter = new EffectFilter { Name = "NewEffect" };
            IList<Effect> effectsBeforeDelete = await _effectRepository.GetByFilterAsync(filter);
            Effect effect = effectsBeforeDelete.SingleOrDefault();

            // Act 
            ValidationResultList validationResult = await _effectRepository.DeleteAsync(effect.Id);

            // Assert
            IList<Effect> effects = await _effectRepository.GetAllAsync(new EffectFilter());
            IList<Effect> effectsAfterDelete = await _effectRepository.GetByFilterAsync(filter);

            Assert.AreEqual(1, effectsBeforeDelete.Count);
            Assert.AreEqual(0, effectsAfterDelete.Count);
            Assert.AreEqual(6, effects.Count);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteAsync_GivenNoMatchingId_ShouldNotDelete_Effect_FromDatabase()
        {
            // Arrange
            await PrepareDatabaseWithEffects();
            IList<Effect> effectsBeforeDelete = await _effectRepository.GetAllAsync(new EffectFilter());

            ObjectId id = ObjectId.GenerateNewId();

            // Act 
            ValidationResultList validationResult = await _effectRepository.DeleteAsync(id.ToString());

            // Assert
            string validationResultErrorMessage = validationResult.Errors.SingleOrDefault().Value;
            IList<Effect> effectsAfterDelete = await _effectRepository.GetAllAsync(new EffectFilter());

            Assert.AreEqual(effectsBeforeDelete.Count, effectsAfterDelete.Count);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.AreEqual(string.Format(Resources.Error_NoEntityWithIdDeleted, id), validationResultErrorMessage);
        }

        [Test]
        public void DeleteAsync_GivenIdIsNull_Throws_ArgumentNullException()
        {
            // Arrange
            string id = null;

            // Assert
            Assert.That(() => _effectRepository.DeleteAsync(id), Throws.TypeOf<ArgumentNullException>());
        }

        private async Task PrepareDatabaseWithEffects()
        {
            // Inserts Effects in DB for testing.
            await _effectRepository.InsertAsync(new Effect { Name = "NewEffect", Details = "newdetail" });
            await _effectRepository.InsertAsync(new Effect { Name = "new", Details = "NewDetail" });
            await _effectRepository.InsertAsync(new Effect { Name = "Effect"});
            await _effectRepository.InsertAsync(new Effect { Name = "XyzEffect"});
            await _effectRepository.InsertAsync(new Effect { Name = "another effect", Details = "Some Detail"});
            await _effectRepository.InsertAsync(new Effect { Name = "effects"});
            await _effectRepository.InsertAsync(new Effect { Name = "anothereffect"});

        }
    }
}