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
    public class MoleculeControllerTest
    {
        private MoleculeController _moleculeController;

        private Mock<IMoleculeService> _moleculeServiceMock;
        private Mock<ISubstanceService> _substanceServiceMock;
        private Mock<IEssentialOilMoleculeService> _essentialOilMoleculeServiceMock;
        private Mock<IEffectMoleculeService> _effectMoleculeServiceMock;

        private IList<Molecule> _moleculeList;
        private IList<Substance> _substanceList;

        [SetUp]
        public void SetUp()
        {
            _moleculeServiceMock = new Mock<IMoleculeService>();
            _substanceServiceMock = new Mock<ISubstanceService>();
            _essentialOilMoleculeServiceMock = new Mock<IEssentialOilMoleculeService>();
            _effectMoleculeServiceMock = new Mock<IEffectMoleculeService>();

            _moleculeController = new MoleculeController(_moleculeServiceMock.Object, _substanceServiceMock.Object, _essentialOilMoleculeServiceMock.Object, _effectMoleculeServiceMock.Object);

            // Set up data
            _moleculeList = CreateMoleculeList();
            _substanceList = CreateSubstanceList();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Index_GivenListOfMolecules_ShouldReturn_ListOfMolecules()
        {
            // Arrange
            _moleculeServiceMock.Setup(m => m.GetAllAsync(It.IsAny<MoleculeFilter>())).Returns(Task.FromResult(_moleculeList));

            // Mock multiple calls of same method.
            _moleculeServiceMock.SetupSequence(m => m.GetSubstanceForMoleculeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_substanceList[0]))
                .Returns(Task.FromResult(_substanceList[1]))
                .Returns(Task.FromResult(_substanceList[2]))
                .Returns(Task.FromResult(_substanceList[3]));

            // Act 
            ViewResult result = await _moleculeController.Index(null) as ViewResult;
            MoleculeViewModelIndex resultModel = result.Model as MoleculeViewModelIndex;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, resultModel.MoleculeViewModels.Count);
            _moleculeServiceMock.Verify(m => m.GetSubstanceForMoleculeAsync(It.IsAny<string>()), Times.Exactly(4));
        }

        [Test]
        public async Task Index_GivenSubstanceAromaticAldehyde_ShouldReturn_MoleculeAromaticAldehyde()
        {
            // Arrange
            _moleculeServiceMock.Setup(m => m.GetAllAsync(It.IsAny<MoleculeFilter>())).Returns(Task.FromResult(_moleculeList));

            // Mock multiple calls of same method.
            _moleculeServiceMock.SetupSequence(m => m.GetSubstanceForMoleculeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_substanceList[0]))
                .Returns(Task.FromResult(_substanceList[1]))
                .Returns(Task.FromResult(_substanceList[2]))
                .Returns(Task.FromResult(_substanceList[3]));

            // Act 
            ViewResult result = await _moleculeController.Index(null) as ViewResult;
            MoleculeViewModelIndex resultModel = result.Model as MoleculeViewModelIndex;

            // Assert
            Assert.IsNotNull(result);
            
            Assert.AreEqual("AromaticAldehyde", resultModel.MoleculeViewModels[0].SubstanceValue);
            Assert.AreEqual("Aromatische Aldehyde allgemein", resultModel.MoleculeViewModels[0].Name);
            Assert.AreEqual("Aromatische Aldehyde", resultModel.MoleculeViewModels[0].SubstanceValueDisplayName);
            _moleculeServiceMock.Verify(m => m.GetSubstanceForMoleculeAsync(It.IsAny<string>()), Times.Exactly(4));
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateMolecule_ShouldRedirectToAction_Index()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Name = "NewMolecule" };
            _moleculeServiceMock.Setup(m => m.InsertAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _moleculeController.CreateOrEdit(moleculeViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _moleculeServiceMock.Verify(m => m.InsertAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewMoleculeWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Name = "NewMolecule" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _moleculeServiceMock.Setup(m => m.InsertAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _moleculeController.CreateOrEdit(moleculeViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _moleculeServiceMock.Verify(m => m.InsertAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewMoleculeWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Name = "NewMolecule" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _moleculeServiceMock.Setup(m => m.InsertAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(validationResult));

            // Act
            await _moleculeController.CreateOrEdit(moleculeViewModel);

            // Assert
            Assert.IsFalse(_moleculeController.ModelState.IsValid);
            _moleculeServiceMock.Verify(m => m.InsertAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditMolecule_ShouldRedirectToAction_Index()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Id = "5a339b56f36d281276999f42", Name = "NewMolecule" };
            _moleculeServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _moleculeController.CreateOrEdit(moleculeViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _moleculeServiceMock.Verify(m => m.UpdateAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditMoleculeWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Id = "5a339b56f36d281276999f42", Name = "NewMolecule" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _moleculeServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _moleculeController.CreateOrEdit(moleculeViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _moleculeServiceMock.Verify(m => m.UpdateAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditMoleculeWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Id = "5a339b56f36d281276999f42", Name = "NewMolecule" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _moleculeServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Molecule>())).Returns(Task.FromResult(validationResult));

            // Act
            await _moleculeController.CreateOrEdit(moleculeViewModel);

            // Assert
            Assert.IsFalse(_moleculeController.ModelState.IsValid);
            _moleculeServiceMock.Verify(m => m.UpdateAsync(It.IsAny<Molecule>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteExistingMolecule_ShouldReturn_EmptyResult()
        {
            // Arrange
            string id = "idString";
            _moleculeServiceMock.Setup(m => m.DeleteMoleculeWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            EmptyResult result = await _moleculeController.Delete(id) as EmptyResult;
            
            // Assert
            Assert.IsNotNull(result);
            _moleculeServiceMock.Verify(m => m.DeleteMoleculeWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteMoleculeWithValidationResultError_ShouldReturn_JsonErrorResultWithErrorMessage()
        {
            // Arrange
            MoleculeViewModel moleculeViewModel = new MoleculeViewModel { Id = "5a339b56f36d281276999f42", Name = "NewMolecule" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _moleculeServiceMock.Setup(m => m.DeleteMoleculeWithAssignmentsAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            JsonErrorResult result = await _moleculeController.Delete(moleculeViewModel.Id) as JsonErrorResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{ errorMessage = new error message }", result.Data.ToString());
            _moleculeServiceMock.Verify(m => m.DeleteMoleculeWithAssignmentsAsync(It.IsAny<string>()), Times.Once);
        }

        private IList<Molecule> CreateMoleculeList()
        {
            IList<Molecule> moleculeList = new List<Molecule>
            {
                new Molecule { Name = "AromaticAldehyde", IsGeneral  = true, Id = "5a339b56f36d281276999f42", SubstanceIdString = "5a339b56f36d281276999f46"},
                new Molecule { Name = "AromaticEster ", IsGeneral  = true, Id = "5a339b56f36d281276999f43", SubstanceIdString = "5a339b56f36d281276999f47"},
                new Molecule { Name = "Cumarine ", IsGeneral  = true, Id = "5a339b56f36d281276999f44", SubstanceIdString = "5a339b56f36d281276999f48"},
                new Molecule { Name = "Phenole ", IsGeneral  = true, Id = "5a339b56f36d281276999f45", SubstanceIdString = "5a339b56f36d281276999f49"},
            };

            return moleculeList;
        }

        private IList<Substance> CreateSubstanceList()
        {
            IList<Substance> substanceList = new List<Substance>
            {
                new Substance { Name = "AromaticAldehyde", Id = "5a339b56f36d281276999f46"},
                new Substance { Name = "AromaticEster ", Id = "5a339b56f36d281276999f47"},
                new Substance { Name = "Cumarine ", Id = "5a339b56f36d281276999f48"},
                new Substance { Name = "Phenole ", Id = "5a339b56f36d281276999f49"},
            };

            return substanceList;
        }
    }
}
