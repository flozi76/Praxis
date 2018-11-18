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
    public class EssentialOilMoleculeServiceTest
    {
        private IEssentialOilMoleculeService _essentialOilMoleculeService;

        private Mock<IEssentialOilMoleculeRepository> _essentialOilMoleculeRepositoryMock;


        [SetUp]
        public void SetUp()
        {
            _essentialOilMoleculeRepositoryMock = new Mock<IEssentialOilMoleculeRepository>();
            _essentialOilMoleculeService = new EssentialOilMoleculeService( _essentialOilMoleculeRepositoryMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenNoMoleculeAssigned_ShouldCall_DeleteAsync_TimesNever()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule>();
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));

            // Act
            await _essentialOilMoleculeService.DeleteAssignedMoleculesAsync(essentialOilId);

            // Assert
            _essentialOilMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenOneMoleculeAssigned_ShouldCall_DeleteAsync_TimesOnce()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule> { new EssentialOilMolecule() };
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));

            // Act
            await _essentialOilMoleculeService.DeleteAssignedMoleculesAsync(essentialOilId);

            // Assert
            _essentialOilMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteAssignedMoleculesAsync_GivenTwoMoleculeAssigned_ShouldCall_DeleteAsync_TimesTwice()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule> { new EssentialOilMolecule(), new EssentialOilMolecule() };
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));

            // Act
            await _essentialOilMoleculeService.DeleteAssignedMoleculesAsync(essentialOilId);

            // Assert
            _essentialOilMoleculeRepositoryMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
