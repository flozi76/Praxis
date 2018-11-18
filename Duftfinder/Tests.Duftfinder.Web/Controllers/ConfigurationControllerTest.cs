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
using Duftfinder.Web.Models;

namespace Tests.Duftfinder.Web.Controllers
{
    [TestFixture]
    public class ConfigurationControllerTest
    {
        private ConfigurationController _configurationController;

        private Mock<IConfigurationService> _configurationServiceMock;

        [SetUp]
        public void SetUp()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();

            _configurationController = new ConfigurationController(_configurationServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Index_GivenListOfConfigurations_ShouldReturn_ListOfConfigurations()
        {
            // Arrange
            IList<Configuration> configurationList = new List<Configuration> { new Configuration { Key = "key", Value = "value" } };
            _configurationServiceMock.Setup(e => e.GetAllAsync(It.IsAny<ConfigurationFilter>())).Returns(Task.FromResult(configurationList));

            // Act 
            ViewResult result = await _configurationController.Index() as ViewResult;
            IList<ConfigurationViewModel> resultModel = result.Model as IList<ConfigurationViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, resultModel.Count);
        }


        [Test]
        public async Task CreateOrEdit_GivenEditConfiguration_ShouldRedirectToAction_Index()
        {
            // Arrange
            ConfigurationViewModel configurationViewModel = new ConfigurationViewModel { Id = "5a339b56f36d281276999f42" };
            _configurationServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Configuration>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _configurationController.Edit(configurationViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _configurationServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Configuration>()), Times.Once);
        }

        [Test]
        public async Task Edit_GivenEditConfigurationWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            ConfigurationViewModel configurationViewModel = new ConfigurationViewModel { Id = "5a339b56f36d281276999f42" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _configurationServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Configuration>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _configurationController.Edit(configurationViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ViewName);
            _configurationServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Configuration>()), Times.Once);
        }

        [Test]
        public async Task Edit_GivenEditConfigurationWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            ConfigurationViewModel configurationViewModel = new ConfigurationViewModel { Id = "5a339b56f36d281276999f42" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _configurationServiceMock.Setup(e => e.UpdateAsync(It.IsAny<Configuration>())).Returns(Task.FromResult(validationResult));

            // Act
            await _configurationController.Edit(configurationViewModel);

            // Assert
            Assert.IsFalse(_configurationController.ModelState.IsValid);
            _configurationServiceMock.Verify(e => e.UpdateAsync(It.IsAny<Configuration>()), Times.Once);
        }
    }
}
