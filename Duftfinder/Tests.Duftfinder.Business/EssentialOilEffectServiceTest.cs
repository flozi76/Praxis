using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using NUnit.Framework;
using Moq;

using Duftfinder.Business.Services;
using Duftfinder.Domain.Helpers;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class EssentialOilEffectServiceTest
    {
        private IEssentialOilEffectService _essentialOilEffectService;

        private Mock<IEssentialOilEffectRepository> _essentialOilEffectRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _essentialOilEffectRepositoryMock = new Mock<IEssentialOilEffectRepository>();

            _essentialOilEffectService = new EssentialOilEffectService(_essentialOilEffectRepositoryMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task DeleteAssignedEssentialOilsAsync_GivenNoEssentialOilAssigned_ShouldCall_DeleteAsync_TimesNever()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOileffectList = new List<EssentialOilEffect>();
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOileffectList));

            // Act
            await _essentialOilEffectService.DeleteAssignedEssentialOilsAsync(effectId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteAssignedEssentialOilsAsync_GivenOneEssentialOilAssigned_ShouldCall_DeleteAsync_TimesOnce()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect() };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            // Act
            await _essentialOilEffectService.DeleteAssignedEssentialOilsAsync(effectId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteAssignedEssentialOilsAsync_GivenTwoEssentialOilAssigned_ShouldCall_DeleteAsync_TimesTwice()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect(), new EssentialOilEffect() };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            // Act
            await _essentialOilEffectService.DeleteAssignedEssentialOilsAsync(effectId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public async Task DeleteAssignedEffectsAsync_GivenNoEffectAssigned_ShouldCall_DeleteAsync_TimesNever()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect>();
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilEffectService.DeleteAssignedEffectsAsync(essentialOilId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteAssignedEffectsAsync_GivenOneEffectsAssigned_ShouldCall_DeleteAsync_TimesOnce()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect() };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilEffectService.DeleteAssignedEffectsAsync(essentialOilId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteAssignedEffectsAsync_GivenTwoEffectsAssigned_ShouldCall_DeleteAsync_TimesTwice()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect(), new EssentialOilEffect() };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilEffectService.DeleteAssignedEffectsAsync(essentialOilId);

            // Assert
            _essentialOilEffectRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
