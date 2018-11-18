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
    public class EffectMoleculeServiceTest
    {
        private IEffectMoleculeService _effectMoleculeService;

        private Mock<IEffectMoleculeRepository> _effectMoleculeRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _effectMoleculeRepositoryMock = new Mock<IEffectMoleculeRepository>();
            _effectMoleculeService = new EffectMoleculeService( _effectMoleculeRepositoryMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenNoMoleculeAssigned_ShouldCall_DeleteAsync_TimesNever()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EffectMolecule> effectMoleculeList = new List<EffectMolecule>();
            _effectMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMoleculeList));
            
            // Act
            await _effectMoleculeService.DeleteAssignedMoleculesAsync(effectId);

            // Assert
            _effectMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>()), Times.Once);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenOneMoleculeAssigned_ShouldCall_DeleteAsync_TimesOnce()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EffectMolecule> effectMoleculeList = new List<EffectMolecule> { new EffectMolecule() };
            _effectMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMoleculeList));

            // Act
            await _effectMoleculeService.DeleteAssignedMoleculesAsync(effectId);

            // Assert
            _effectMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>()), Times.Once);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenTwoMoleculeAssigned_ShouldCall_DeleteAsync_TimesTwice()
        {
            // Arrange
            string effectId = "5a339b56f36d281276999f42";
            IList<EffectMolecule> effectMoleculeList = new List<EffectMolecule> { new EffectMolecule(), new EffectMolecule() };
            _effectMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMoleculeList));

            // Act
            await _effectMoleculeService.DeleteAssignedMoleculesAsync(effectId);

            // Assert
            _effectMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EffectMoleculeFilter>()), Times.Once);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
