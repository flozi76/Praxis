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
    public class SearchEssentialOilControllerTest
    {
        private SearchEssentialOilController _searchEssentialOilController;

        private Mock<IEssentialOilService> _essentialOilServiceMock;

        [SetUp]
        public void SetUp()
        {
            _essentialOilServiceMock = new Mock<IEssentialOilService>();

            _searchEssentialOilController = new SearchEssentialOilController(_essentialOilServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }
        
        [Test]
        public async Task EssentialOilDetails_GivenEssentialOil_ShouldShowView_EssentialOilDetails()
        {
            // Arrange
            string essentialOilId = "idString";

            EssentialOil essentialOil = new EssentialOil();
            _essentialOilServiceMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(essentialOil));

            // Act
            PartialViewResult result = await _searchEssentialOilController.EssentialOilDetails(essentialOilId, "Such Text") as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_EssentialOilDetails", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task GetEssentialOilNames_GivenEssentialOils_ShouldReturnArrayOfNamesOf_EssentialOils()
        {
            // Arrange
            IList<EssentialOil> essentialOilList = new List<EssentialOil> { new EssentialOil { Name = "TestOil" }, new EssentialOil { Name = "NewOil" }, };
            _essentialOilServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(essentialOilList));

            // Act
            JsonResult result = await _searchEssentialOilController.GetEssentialOilNames();
            string[] arrayResult = result.Data as string[];
            
            // Assert
            Assert.IsNotNull(arrayResult);
            Assert.AreEqual(2, arrayResult.Length);
            Assert.AreEqual(arrayResult[0], "TestOil");
            Assert.AreEqual(arrayResult[1], "NewOil");
            _essentialOilServiceMock.Verify(e => e.GetAllAsync(It.IsAny<EssentialOilFilter>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenEssentialOil_ShouldShowPartialView_EssentialOilSearchResults()
        {
            // Arrange
            IList<EssentialOil> essentialOilList = new List<EssentialOil>();
            _essentialOilServiceMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(essentialOilList));

            // Act
            PartialViewResult result = await _searchEssentialOilController.SearchEssentialOil(new SearchEssentialOilViewModel()) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Views/SearchEssentialOil/_EssentialOilSearchResults.cshtml", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilFilter>()), Times.Once);
        }
    }
}
