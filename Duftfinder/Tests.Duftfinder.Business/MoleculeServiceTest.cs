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
    public class MoleculeServiceTest
    {
        private IMoleculeService _moleculeService;

        private Mock<IMoleculeRepository> _moleculeRepositoryMock;
        private Mock<IEssentialOilMoleculeRepository> _essentialOilMoleculeRepositoryMock;
        private Mock<IEffectMoleculeRepository> _effectMoleculeRepositoryMock;
        private Mock<ISubstanceRepository> _substanceRepositoryMock;

        private IList<Molecule> _moleculeList;

        [SetUp]
        public void SetUp()
        {
            _moleculeRepositoryMock = new Mock<IMoleculeRepository>();
            _essentialOilMoleculeRepositoryMock = new Mock<IEssentialOilMoleculeRepository>();
            _effectMoleculeRepositoryMock = new Mock<IEffectMoleculeRepository>();
            _substanceRepositoryMock = new Mock<ISubstanceRepository>();
            _moleculeService = new MoleculeService(_moleculeRepositoryMock.Object, _essentialOilMoleculeRepositoryMock.Object, _effectMoleculeRepositoryMock.Object, _substanceRepositoryMock.Object);

            // Set up data
            _moleculeList = CreateMoleculeList();

        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public void GetAllAsync_GivenListOfMolecules_ShouldReturn_ListOfMolecules()
        {
            // Arrange
            _moleculeRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<MoleculeFilter>())).Returns(Task.FromResult(_moleculeList));
            MoleculeFilter filter = new MoleculeFilter();

            // Act 
            Task<IList<Molecule>> result = _moleculeService.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(4, result.Result.Count);
            Assert.AreEqual(_moleculeList, result.Result);
        }

        [Test]
        public void GetByIdAsync_GivenIdOfMolecule_ShouldReturn_Molecule()
        {
            // Arrange
            string id = "idString";
            Molecule molecule = new Molecule { Name = "NewMolecule" };
            _moleculeRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(molecule));

            // Act 
            Task<Molecule> result = _moleculeService.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(molecule, result.Result);
        }

        [Test]
        public void InsertAsync_GivenNewMolecule_ShouldInsert_NewMolecule()
        {
            // Arange 
            Molecule molecule = new Molecule { Name = "NewMolecule" };

            // Act 
            Task<ValidationResultList> validationResult = _moleculeService.InsertAsync(molecule);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.InsertAsync(It.IsAny<Molecule>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void UpdateAsync_GivenNewMolecule_ShouldUpdate_NewMolecule()
        {
            // Arange 
            Molecule molecule = new Molecule { Name = "NewMolecule" };

            // Act 
            Task<ValidationResultList> validationResult = _moleculeService.UpdateAsync(molecule);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.UpdateAsync(It.IsAny<Molecule>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenMoleculeId_ShouldDelete_Molecule()
        {
            // Arange 
            string id = "idString";

            // Act 
            Task<ValidationResultList> validationResult = _moleculeService.DeleteAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenMoleculeIdIsNull_ShouldNotDelete_Molecule()
        {
            // Arange 
            string id = null;

            // Act 
            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Throws(new ArgumentNullException());

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteMoleculeWithAssignmentsAsync_GivenMoleculeIdWithNoAssignments_ShouldDelete_Molecule()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule>();
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule>();

            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules)); 

            // Act 
            validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never); 
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteMoleculeWithAssignmentsAsync_GivenMoleculeIdWithEssentialOilAndEffectAssigned_ShouldDelete_MoleculeAndAssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule(), new EssentialOilMolecule() };
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult)); 

            // Act 
            validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteMoleculeWithAssignmentsAsync_GivenMoleculeIdWithEssentialOilAssigned_ShouldDelete_MoleculeAndAssignedEssentialOil()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule()};
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule>();

            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteMoleculeWithAssignmentsAsync_GivenMoleculeIdWithEffectAssigned_ShouldDelete_MoleculeAndAssignedEffect()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule>();
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteMoleculeWithAssignmentsAsync_GivenDeleteMoleculeWithValidationResultError_ShouldNotDelete_AssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule(), new EssentialOilMolecule() };
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _moleculeRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));

            // Act 
            validationResult = await _moleculeService.DeleteMoleculeWithAssignmentsAsync(id);

            // Assert
            _moleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.IsTrue(validationResult.Errors.ContainsKey("errorKey"));
        }

        [Test]
        public async Task GetSubstanceForMoleculeAsync_GivenSubstanceId_ShouldReturn_Substance()
        {
            // Arrange
            string substanceId = "5a339b56f36d281276999f42";
            Substance substance = new Substance();
            _substanceRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(substance));

            // Act
            Substance result = await _moleculeService.GetSubstanceForMoleculeAsync(substanceId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(substance, result);
            _substanceRepositoryMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        private IList<Molecule> CreateMoleculeList()
        {
            IList<Molecule> moleculeList = new List<Molecule>
            {
                new Molecule { Name = "AromaticAldehyde", IsGeneral  = true, Id = "5a339b56f36d281276999f42"},
                new Molecule { Name = "AromaticEster ", IsGeneral  = true, Id = "5a339b56f36d281276999f43"},
                new Molecule { Name = "Cumarine ", IsGeneral  = true, Id = "5a339b56f36d281276999f44"},
                new Molecule { Name = "Phenole ", IsGeneral  = true, Id = "5a339b56f36d281276999f44"},
            };

            return moleculeList;
        }
    }
}
