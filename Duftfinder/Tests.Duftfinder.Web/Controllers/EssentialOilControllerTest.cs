using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using NUnit.Framework;
using Duftfinder.Web.Controllers;
using Moq;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Helpers;
using Duftfinder.Web.Models;

namespace Tests.Duftfinder.Web.Controllers
{
    [TestFixture]
    public class EssentialOilControllerTest
    {
        private EssentialOilController _essentialOilController;

        private Mock<IEssentialOilService> _essentialOilServiceMock;
        private Mock<IMoleculeService> _moleculeServiceMock;
        private Mock<ISubstanceService> _substanceServiceMock;
        private Mock<IEssentialOilMoleculeService> _essentialOilMoleculeServiceMock;
        private Mock<IEffectService> _effectServiceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IEssentialOilEffectService> _essentialOilEffectServiceMock;

        private IList<EssentialOil> _essentialOilList;

        [SetUp]
        public void SetUp()
        {
            _essentialOilServiceMock = new Mock<IEssentialOilService>();
            _moleculeServiceMock = new Mock<IMoleculeService>();
            _substanceServiceMock = new Mock<ISubstanceService>();
            _essentialOilMoleculeServiceMock = new Mock<IEssentialOilMoleculeService>();
            _effectServiceMock = new Mock<IEffectService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _essentialOilEffectServiceMock = new Mock<IEssentialOilEffectService>();

            _essentialOilController = new EssentialOilController(_essentialOilServiceMock.Object, _moleculeServiceMock.Object, _substanceServiceMock.Object, _essentialOilMoleculeServiceMock.Object, _effectServiceMock.Object, _categoryServiceMock.Object, _essentialOilEffectServiceMock.Object);

            // Set up data
            _essentialOilList = CreateEssentialOils();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Index_GivenListOfEssentialOils_ShouldReturn_ListOfEssentialOils()
        {
            // Arrange
            _essentialOilServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(_essentialOilList));

            // Act 
            ViewResult result = await _essentialOilController.Index(null) as ViewResult;
            EssentialOilViewModelIndex resultModel = result.Model as EssentialOilViewModelIndex;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, resultModel.EssentialOilViewModels.Count);
            Assert.AreEqual(3, resultModel.AlphabeticalIndexes.Count);
            Assert.AreEqual("P", resultModel.AlphabeticalIndexes[0]);
            Assert.AreEqual("L", resultModel.AlphabeticalIndexes[1]);
            Assert.AreEqual("S", resultModel.AlphabeticalIndexes[2]);
        }

        [Test]
        public void AssignEffect_GivenListOfAssignValueViewModelIsNull_Throws_ArgumentException()
        {
            // Assert
            Assert.That(() => _essentialOilController.AssignEffect(new AssignEssentialOilEffectViewModel(), null), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void AssignEffect_GivenEssentialOilEffectViewModelHasNoEssentialOilId_Throws_ArgumentException()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel();
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Assert
            Assert.That(() => _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public async Task AssignEffect_GivenOneEffectAssigned_ShouldCall_DeleteAssignedEffectsAsync_TimesOnce()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AssignEffect_GivenAssignNoEffect_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEffect_GivenAssignEffect_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Once);
        }

        [Test]
        public async Task AssignEffect_GivenAssignOneEffectDegree0_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 0 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEffect_GivenAssignEffectDegreeNegative_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = -1 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            
            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEffect_GivenAssignTowEffect_ShouldCall_InsertAsync_TimesTwice()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 }, new AssignValueViewModel { EffectDegree = 4 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Exactly(2));
        }

        [Test]
        public async Task AssignEffect_GivenAssignNoEffect_ShouldRedirectToAction_Index()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            
            // Act
            RedirectToRouteResult result = await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignEffect_GivenAssignOneEffect_ShouldRedirectToAction_Index()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignEffect_GivenAssignEffectToEssentialOilReturnsValidationResultError_ShouldShowView_AssignEffect()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEffect", result.ViewName);
        }

        [Test]
        public async Task AssignEffect_GivenDeleteAssignedEffectsReturnsValidationResultError_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEffect_GivenDeleteAssignedEffectsReturnsValidationResultError_ShouldShowView_AssignEffect()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEffectsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.AssignEffect(assignEssentialOilEffectViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEffect", result.ViewName);
        }

        [Test]
        public void AssignMolecule_GivenListOfAssignValueViewModelIsNull_Throws_ArgumentNullException()
        {
            // Assert
            Assert.That(() => _essentialOilController.AssignMolecule(new AssignMoleculeViewModel(), null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void AssignMolecule_GivenAssignMoleculeViewModelHasNoEssentialOilId_Throws_ArgumentNullException()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel();
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Assert
            Assert.That(() => _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task AssignMolecule_GivenOneMoleculeAssigned_ShouldCall_DeleteAssignedMoleculesAsync_TimesOnce()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignNoMolecule_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            
            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMolecule_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Once);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMolecule0_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMoleculeNegative_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = -0.5 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignTowMolecule_ShouldCall_InsertAsync_TimesTwice()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 }, new AssignValueViewModel { MoleculePercentage = 87 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Exactly(2));
        }

        [Test]
        public async Task AssignMolecule_GivenAssignNoMolecule_ShouldRedirectToAction_AssignEffect()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEffect", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignOneMolecule_ShouldRedirectToAction_AssignEffect()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEffect", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMoleculeToEssentialOilReturnsValidationResultError_ShouldShowView_AssignMolecule()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 } };
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            ValidationResultList validationResult = new ValidationResultList {Errors = { new KeyValuePair<string, string>("errorKey", "new error message") }};
            _essentialOilMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.ViewName);
        }

        [Test]
        public async Task AssignMolecule_GivenDeleteAssignedMoleculesReturnsValidationResultError_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 } };
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule> { new EssentialOilMolecule { Id = "5a339b56f36d281276999f43" } };
            _essentialOilMoleculeServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));
            ValidationResultList validationResult = new ValidationResultList {Errors = { new KeyValuePair<string, string>("errorKey", "new error message") }};
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _essentialOilMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenDeleteAssignedMoleculesReturnsValidationResultError_ShouldShowView_AssignMolecule()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EssentialOilId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { MoleculePercentage = 0.5 } };
            ValidationResultList validationResult = new ValidationResultList {Errors = { new KeyValuePair<string, string>("errorKey", "new error message") }};
            _essentialOilMoleculeServiceMock.Setup(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.ViewName);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEssentialOil_ShouldRedirectToAction_AssignMolecule()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Name = "NewEssentialOil" };
            _essentialOilServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _essentialOilController.CreateOrEdit(essentialOilViewModel, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.RouteValues["action"]);
            _essentialOilServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEssentialOilWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Name = "NewEssentialOil" };
            ValidationResultList validationResult = new ValidationResultList {Errors = { new KeyValuePair<string, string>("errorKey", "new error message") }};
            _essentialOilServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.CreateOrEdit(essentialOilViewModel, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEssentialOilWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Name = "NewEssentialOil" };
            ValidationResultList validationResult = new ValidationResultList {Errors = { new KeyValuePair<string, string>("errorKey", "new error message") }};
            _essentialOilServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(validationResult));

            // Act
            await _essentialOilController.CreateOrEdit(essentialOilViewModel, null);

            // Assert
            Assert.IsFalse(_essentialOilController.ModelState.IsValid);
            _essentialOilServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEssentialOilWithPicture_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            string fileName = "testimage.jpg";
            // Get the executing directory of the tests 
            string dir = TestContext.CurrentContext.TestDirectory;
            // Infer the project directory from there...2 levels up.
            dir = Directory.GetParent(dir).Parent?.FullName;
            string filePath = $"{dir}\\TestData\\{fileName}";

            Stream stream = new FileStream(filePath, FileMode.Open);
            MockPostedFileBase uploadFile = new MockPostedFileBase(stream, "", fileName);

            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Name = "NewEssentialOil" };
            _essentialOilServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(new ValidationResultList()));
            IList<EssentialOil> essentialOilList = new List<EssentialOil> { new EssentialOil { Id = "5a339b56f36d281276999f42" } };
            _essentialOilServiceMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(essentialOilList));

            // Act
            await _essentialOilController.CreateOrEdit(essentialOilViewModel, uploadFile);

            _essentialOilServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEssentialOil_ShouldRedirectToAction_AssignMolecule()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEssentialOil" };
            _essentialOilServiceMock.Setup(e => e.UpdateAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _essentialOilController.CreateOrEdit(essentialOilViewModel, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.RouteValues["action"]);
            _essentialOilServiceMock.Verify(e => e.UpdateAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEssentialOilWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEssentialOil" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilServiceMock.Setup(e => e.UpdateAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _essentialOilController.CreateOrEdit(essentialOilViewModel, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.UpdateAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEssentialOilWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEssentialOil" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilServiceMock.Setup(e => e.UpdateAsync(It.IsAny<EssentialOil>())).Returns(Task.FromResult(validationResult));

            // Act
            await _essentialOilController.CreateOrEdit(essentialOilViewModel, null);

            // Assert
            Assert.IsFalse(_essentialOilController.ModelState.IsValid);
            _essentialOilServiceMock.Verify(e => e.UpdateAsync(It.IsAny<EssentialOil>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteExistingEssentialOil_ShouldReturn_EmptyResult()
        {
            // Arrange
            string id = "idString";
            _essentialOilServiceMock.Setup(e => e.DeleteEssentialOilWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            EmptyResult result = await _essentialOilController.Delete(id) as EmptyResult;

            // Assert
            Assert.IsNotNull(result);
            _essentialOilServiceMock.Verify(e => e.DeleteEssentialOilWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteEssentialOilWithValidationResultError_ShouldReturn_JsonErrorResultWithErrorMessage()
        {
            // Arrange
            EssentialOilViewModel essentialOilViewModel = new EssentialOilViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEssentialOil" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilServiceMock.Setup(e => e.DeleteEssentialOilWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            JsonErrorResult result = await _essentialOilController.Delete(essentialOilViewModel.Id) as JsonErrorResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{ errorMessage = new error message }", result.Data.ToString());
            _essentialOilServiceMock.Verify(e => e.DeleteEssentialOilWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        private IList<EssentialOil> CreateEssentialOils()
        {
            IList<EssentialOil> essentialOilList = new List<EssentialOil>
            {
                new EssentialOil {Name = "Pfefferminze", Description = "Pfefferminze Description"},
                new EssentialOil {Name = "Lavendel", Description = "Lavendel Description"},
                new EssentialOil {Name = "Sandholz", Description = "Sandholz Description"},
            };

            return essentialOilList;
        }
    }
}
