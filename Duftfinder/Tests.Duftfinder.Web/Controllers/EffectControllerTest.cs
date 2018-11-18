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
    public class EffectControllerTest
    {
        private EffectController _effectController;

        private Mock<IEffectService> _effectServiceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IMoleculeService> _moleculeServiceMock;
        private Mock<ISubstanceService> _substanceServiceMock;
        private Mock<IEffectMoleculeService> _effectMoleculeServiceMock;
        private Mock<IEssentialOilEffectService> _essentialOilEffectServiceMock;
        private Mock<IEssentialOilService> _essentialOilServiceMock;

        private IList<Effect> _effectList;
        private IList<Category> _categoryList;

        [SetUp]
        public void SetUp()
        {
            _effectServiceMock = new Mock<IEffectService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _moleculeServiceMock = new Mock<IMoleculeService>();
            _substanceServiceMock = new Mock<ISubstanceService>();
            _effectMoleculeServiceMock = new Mock<IEffectMoleculeService>();
            _effectServiceMock = new Mock<IEffectService>();
            _essentialOilEffectServiceMock = new Mock<IEssentialOilEffectService>();
            _essentialOilServiceMock = new Mock<IEssentialOilService>();

            _effectController = new EffectController(_effectServiceMock.Object, _categoryServiceMock.Object, _moleculeServiceMock.Object, _substanceServiceMock.Object, _effectMoleculeServiceMock.Object, _essentialOilEffectServiceMock.Object, _essentialOilServiceMock.Object );

            // Set up data
            _effectList = CreateEffectList();
            _categoryList = CreateCategoryList();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Index_GivenListOfEffects_ShouldReturn_ListOfEffects()
        {
            // Arrange
            _effectServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(_effectList));
            _categoryServiceMock.Setup(e => e.GetAllAsync(It.IsAny<CategoryFilter>())).Returns(Task.FromResult(_categoryList));

            // Act 
            ViewResult result = await _effectController.Index(null) as ViewResult;
            EffectViewModelIndex resultModel = result.Model as EffectViewModelIndex;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultModel);
            Assert.IsNotNull(resultModel.EffectViewModels);
            Assert.AreEqual(5, resultModel.EffectViewModels.Count);
        }

        [Test]
        public async Task Index_GivenListOfEffects_ShouldReturn_ListOfEffectsWithCategoryDisplayItems()
        {
            // Arrange
            _effectServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(_effectList));
            _categoryServiceMock.Setup(e => e.GetAllAsync(It.IsAny<CategoryFilter>())).Returns(Task.FromResult(_categoryList));
            // Act 
            ViewResult result = await _effectController.Index(null) as ViewResult;
            EffectViewModelIndex resultModel = result.Model as EffectViewModelIndex;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultModel);
            Assert.IsNotNull(resultModel.CategoryDisplayItems);
            Assert.AreEqual(4, resultModel.CategoryDisplayItems.Count);
            Assert.AreEqual("Muskeln", resultModel.CategoryDisplayItems[0].Value);
            Assert.AreEqual("Nervensystem allgemein", resultModel.CategoryDisplayItems[1].Value);
            Assert.AreEqual("Schmerzen", resultModel.CategoryDisplayItems[2].Value);
            Assert.AreEqual("Integration Jüngere Selbst", resultModel.CategoryDisplayItems[3].Value);
        }

        [Test]
        public void AssignEssentialOil_GivenListOfAssignValueViewModelIsNull_Throws_ArgumentException()
        {
            // Assert
            Assert.That(() => _effectController.AssignEssentialOil(new AssignEssentialOilEffectViewModel(), null), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void AssignEssentialOil_GivenAssignEssentialOilEffectViewModelHasNoEffectId_Throws_ArgumentException()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel();
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Assert
            Assert.That(() => _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public async Task AssignEssentialOil_GivenOneEssentialOilAssigned_ShouldCall_DeleteAssignedEssentialOilAsync_TimesOnce()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.DeleteAssignedEssentialOilsAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignNoEssentialOil_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignEssentialOil_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect { Id = "5a339b56f36d281276999f43" } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Once);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignOneEffectDegree0_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 0 } };
            _essentialOilEffectServiceMock.Setup(e => e.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            
            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignEffectDegreeNegative_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = -1 } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignTowEssentialOil_ShouldCall_InsertAsync_TimesTwice()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 }, new AssignValueViewModel { EffectDegree = 4 } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Exactly(2));
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignNoEssentialOil_ShouldRedirectToAction_Index()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            
            // Act
            RedirectToRouteResult result = await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignOneEssentialOil_ShouldRedirectToAction_Index()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignEssentialOil_GivenAssignEssentialOilToEffectReturnsValidationResultError_ShouldShowView_AssignEssentialOil()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(e => e.InsertAsync(It.IsAny<EssentialOilEffect>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEssentialOil", result.ViewName);
        }

        [Test]
        public async Task AssignEssentialOil_GivenDeleteAssignedEssentialOilsReturnsValidationResultError_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels);

            // Assert
            _essentialOilEffectServiceMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOilEffect>()), Times.Never);
        }

        [Test]
        public async Task AssignEssentialOil_GivenDeleteAssignedEssentialOilsReturnsValidationResultError_ShouldShowView_AssignEssentialOil()
        {
            // Arrange
            AssignEssentialOilEffectViewModel assignEssentialOilEffectViewModel = new AssignEssentialOilEffectViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _essentialOilEffectServiceMock.Setup(x => x.DeleteAssignedEssentialOilsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _effectController.AssignEssentialOil(assignEssentialOilEffectViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEssentialOil", result.ViewName);
        }

        [Test]
        public void AssignMolecule_GivenListOfAssignValueViewModelIsNull_Throws_ArgumentException()
        {
            // Assert
            Assert.That(() => _effectController.AssignMolecule(new AssignMoleculeViewModel(), null), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void AssignMolecule_GivenAssignMoleculeViewModelHasNoEffectId_Throws_ArgumentException()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel();
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();

            // Assert
            Assert.That(() => _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public async Task AssignMolecule_GivenOneMoleculeAssigned_ShouldCall_DeleteAssignedMoleculesAsync_TimesOnce()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel>();
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.DeleteAssignedMoleculesAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignNoMolecule_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMolecule_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _effectMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EffectMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Once);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignOneEffectDegree0_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 0 } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Never);
        }
        
        [Test]
        public async Task AssignMolecule_GivenAssignEffectDegreeNegative_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = -1 } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignTowMolecule_ShouldCall_InsertAsync_TimesTwice()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 }, new AssignValueViewModel { EffectDegree = 4 } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _effectMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EffectMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Exactly(2));
        }

        [Test]
        public async Task AssignMolecule_GivenAssignNoMolecule_ShouldRedirectToAction_AssignEssentialOil()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEssentialOil", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignOneMolecule_ShouldRedirectToAction_AssignEssentialOil()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2} };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _effectMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EffectMolecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignEssentialOil", result.RouteValues["action"]);
        }

        [Test]
        public async Task AssignMolecule_GivenAssignMoleculeToEffectReturnsValidationResultError_ShouldShowView_AssignMolecule()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectMoleculeServiceMock.Setup(e => e.InsertAsync(It.IsAny<EffectMolecule>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.ViewName);
        }

        [Test]
        public async Task AssignMolecule_GivenDeleteAssignedMoleculesReturnsValidationResultError_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            
            // Act
            await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels);

            // Assert
            _effectMoleculeServiceMock.Verify(e => e.InsertAsync(It.IsAny<EffectMolecule>()), Times.Never);
        }

        [Test]
        public async Task AssignMolecule_GivenDeleteAssignedMoleculesReturnsValidationResultError_ShouldShowView_AssignMolecule()
        {
            // Arrange
            AssignMoleculeViewModel assignMoleculeViewModel = new AssignMoleculeViewModel { EffectId = "5a339b56f36d281276999f42" };
            IList<AssignValueViewModel> assignValueViewModels = new List<AssignValueViewModel> { new AssignValueViewModel { EffectDegree = 2 } };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectMoleculeServiceMock.Setup(x => x.DeleteAssignedMoleculesAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            
            // Act
            ViewResult result = await _effectController.AssignMolecule(assignMoleculeViewModel, assignValueViewModels) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.ViewName);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateEffect_ShouldRedirectToAction_Index()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Name = "NewEffect" };
            _effectServiceMock.Setup(e => e.InsertAsync(It.IsAny<Effect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _effectController.CreateOrEdit(effectViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.RouteValues["action"]);
            _effectServiceMock.Verify(e => e.InsertAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEffectWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Name = "NewEffect" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectServiceMock.Setup(e => e.InsertAsync(It.IsAny<Effect>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _effectController.CreateOrEdit(effectViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _effectServiceMock.Verify(e => e.InsertAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewEffectWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Name = "NewEffect" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectServiceMock.Setup(e => e.InsertAsync(It.IsAny<Effect>())).Returns(Task.FromResult(validationResult));

            // Act
            await _effectController.CreateOrEdit(effectViewModel);

            // Assert
            Assert.IsFalse(_effectController.ModelState.IsValid);
            _effectServiceMock.Verify(e => e.InsertAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEffect_ShouldRedirectToAction_Index()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEffect" };
            _effectServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Effect>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _effectController.CreateOrEdit(effectViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AssignMolecule", result.RouteValues["action"]);
            _effectServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEffectWithValidationResultError_ShouldShowView_CreateOrEdit()
        {

            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEffect" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Effect>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _effectController.CreateOrEdit(effectViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _effectServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditEffectWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEffect" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Effect>())).Returns(Task.FromResult(validationResult));

            // Act
            await _effectController.CreateOrEdit(effectViewModel);

            // Assert
            Assert.IsFalse(_effectController.ModelState.IsValid);
            _effectServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Effect>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteExistingEffect_ShouldReturn_EmptyResult()
        {
            // Arrange
            string id = "idString";
            _effectServiceMock.Setup(e => e.DeleteEffectWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            EmptyResult result = await _effectController.Delete(id) as EmptyResult;

            // Assert
            Assert.IsNotNull(result);
            _effectServiceMock.Verify(e => e.DeleteEffectWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteEffectWithValidationResultError_ShouldReturn_JsonErrorResultWithErrorMessage()
        {
            // Arrange
            EffectViewModel effectViewModel = new EffectViewModel { Id = "5a339b56f36d281276999f42", Name = "NewEffect" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _effectServiceMock.Setup(e => e.DeleteEffectWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            JsonErrorResult result = await _effectController.Delete(effectViewModel.Id) as JsonErrorResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{ errorMessage = new error message }", result.Data.ToString());
            _effectServiceMock.Verify(e => e.DeleteEffectWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        private IList<Effect> CreateEffectList()
        {
            IList<Effect> effectList = new List<Effect>
            {
                new Effect { Name = "Schmerzlindernd: Gelenke", Id = "5a339b56f36d281276999f42", CategoryIdString = "5a339b56f36d281276999f49" },
                new Effect { Name = "Schmerzlindernd: Muskeln", Id = "5a339b56f36d281276999f43", CategoryIdString = "5a339b56f36d281276999f49" },
                new Effect { Name = "Bronchien entkrampfend", Id = "5a339b56f36d281276999f44", CategoryIdString = "5a339b56f36d281276999f47" },
                new Effect { Name = "Angstlösend (anxiolytisch)", Id = "5a339b56f36d281276999f45", CategoryIdString = "5a339b56f36d281276999f48" },
                new Effect { Name = "Seelisch aufhellend, heilend", Id = "5a339b56f36d281276999f46", CategoryIdString = "5a339b56f36d281276999f50" },
            };

            return effectList;
        }

        private IList<Category> CreateCategoryList()
        {
            IList<Category> categoryList = new List<Category>
            {
                new Category { Id = "5a339b56f36d281276999f47", Name = "Muscle" },
                new Category { Id = "5a339b56f36d281276999f48", Name = "NervousSystemGeneral" },
                new Category { Id = "5a339b56f36d281276999f49", Name = "Pain" },
                new Category { Id = "5a339b56f36d281276999f50", Name = "IntegrationYoungerSelf" }
            };
            return categoryList;
        }
    }
}
