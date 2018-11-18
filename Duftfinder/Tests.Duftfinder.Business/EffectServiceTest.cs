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
    public class EffectServiceTest
    {
        private IEffectService _effectService;

        private Mock<IEffectRepository> _effectRepositoryMock;
        private Mock<IEssentialOilEffectRepository> _essentialOilEffectRepositoryMock;
        private Mock<IEffectMoleculeRepository> _effectMoleculeRepositoryMock;

        private IList<Effect> _effectList;

        [SetUp]
        public void SetUp()
        {
            _effectRepositoryMock = new Mock<IEffectRepository>();
            _effectMoleculeRepositoryMock = new Mock<IEffectMoleculeRepository>();
            _essentialOilEffectRepositoryMock = new Mock<IEssentialOilEffectRepository>();

            _effectService = new EffectService(_effectRepositoryMock.Object, _essentialOilEffectRepositoryMock.Object, _effectMoleculeRepositoryMock.Object);

            // Set up data
            _effectList = CreateEffectList();

        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public void GetAllAsync_GivenListOfEffects_ShouldReturn_ListOfEffects()
        {
            // Arrange
            _effectRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(_effectList));
            EffectFilter filter = new EffectFilter();

            // Act 
            Task<IList<Effect>> result = _effectService.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(5, result.Result.Count);
            Assert.AreEqual(_effectList, result.Result);
        }

        [Test]
        public void GetByIdAsync_GivenIdOfEffect_ShouldReturn_Effect()
        {
            // Arrange
            string id = "idString";
            Effect effect = new Effect { Name = "NewEffect" };
            _effectRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(effect));

            // Act 
            Task<Effect> result = _effectService.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(effect, result.Result);
        }

        [Test]
        public void InsertAsync_GivenNewEffect_ShouldInsert_NewEffect()
        {
            // Arange 
            Effect effect = new Effect { Name = "NewEffect" };

            // Act 
            Task<ValidationResultList> validationResult = _effectService.InsertAsync(effect);

            // Assert
            _effectRepositoryMock.Verify(e => e.InsertAsync(It.IsAny<Effect>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void UpdateAsync_GivenNewEffect_ShouldUpdate_NewEffect()
        {
            // Arange 
            Effect effect = new Effect { Name = "NewEffect" };

            // Act 
            Task<ValidationResultList> validationResult = _effectService.UpdateAsync(effect);

            // Assert
            _effectRepositoryMock.Verify(e => e.UpdateAsync(It.IsAny<Effect>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenEffectId_ShouldDelete_Effect()
        {
            // Arange 
            string id = "idString";

            // Act 
            Task<ValidationResultList> validationResult = _effectService.DeleteAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenEffectIdIsNull_ShouldNotDelete_Effect()
        {
            // Arange 
            string id = null;

            // Act 
            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Throws(new ArgumentNullException());

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteEffectWithAssignmentsAsync_GivenEffectIdWithNoAssignments_ShouldDelete_Effect()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect>();
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule>();

            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));

            // Act 
            validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEffectWithAssignmentsAsync_GivenEffectIdWithEssentialOilAndMoleculeAssigned_ShouldDelete_EffectAndAssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect(), new EssentialOilEffect() };
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEffectWithAssignmentsAsync_GivenEffectIdWithEssentialOilAssigned_ShouldDelete_EffectAndAssignedEssentialOil()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect() };
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule>();

            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEffectWithAssignmentsAsync_GivenEffectIdWithMoleculeAssigned_ShouldDelete_EffectAndAssignedMolecule()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect>();
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _effectMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEffectWithAssignmentsAsync_GivenDeleteEffectWithValidationResultError_ShouldNotDelete_AssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect(), new EssentialOilEffect() };
            IList<EffectMolecule> effectMolecules = new List<EffectMolecule> { new EffectMolecule() };

            _effectRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _effectMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectMoleculeFilter>())).Returns(Task.FromResult(effectMolecules));

            // Act 
            validationResult = await _effectService.DeleteEffectWithAssignmentsAsync(id);

            // Assert
            _effectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _effectMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.IsTrue(validationResult.Errors.ContainsKey("errorKey"));
        }

        private IList<Effect> CreateEffectList()
        {
            IList<Effect> effectList = new List<Effect>
            {
                new Effect { Name = "Schmerzlindernd: Gelenke" },
                new Effect { Name = "Schmerzlindernd: Muskeln " },
                new Effect { Name = "Bronchien entkrampfend" },
                new Effect { Name = "Angstlösend (anxiolytisch)"},
                new Effect { Name = "Seelisch aufhellend, heilend"},
            };

            return effectList;
        }
    }
}
