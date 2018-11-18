using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duftfinder.Domain.Dtos;
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
    public class SearchEffectsControllerTest
    {
        private SearchEffectsController _searchEffectsController;

        private Mock<IEssentialOilService> _essentialOilServiceMock;
        private Mock<IEffectService> _effectServiceMock;

        [SetUp]
        public void SetUp()
        {
            _essentialOilServiceMock = new Mock<IEssentialOilService>();
            _effectServiceMock = new Mock<IEffectService>();

            _searchEffectsController = new SearchEffectsController(_essentialOilServiceMock.Object, _effectServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }
        
        [Test]
        public async Task SearchEssentialOil_GivenOneSearchEffects_ShouldSearchFor_SearchEffectsCount1()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, searchEffects.Count);
            Assert.AreEqual(searchEffects.Count, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(1, searchEffects[0].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenTwoSearchEffects_ShouldSearchFor_SearchEffectsCount2()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 2},
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, searchEffects.Count);
            Assert.AreEqual(searchEffects.Count, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(1, searchEffects[0].DiscomfortValue);
            Assert.AreEqual("Wirkung gegen Narben", searchEffects[1].SearchEffectText);
            Assert.AreEqual(2, searchEffects[1].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenNoSearchEffects_ShouldSearchFor_SearchEffectsCount0()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenOneDuplicateSearchEffectWithSameDiscomfortValue_ShouldSearchFor_SearchEffectsCount1()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, searchEffects.Count);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(1, searchEffects[0].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenOneDuplicateSearchEffectWithSameDiscomfortValueAndWhitespace_ShouldSearchFor_SearchEffectsCount1()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen ", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, searchEffects.Count);
            Assert.AreEqual(searchEffects.Count, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(1, searchEffects[0].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenOneDuplicateSearchEffectWithNotSameDiscomfortValue_ShouldSearchFor_SearchEffectsCount1WithHighestDiscomfortValue()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 3},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 2},
                new SearchEffectItem(),
                new SearchEffectItem()
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, searchEffects.Count);
            Assert.AreEqual(searchEffects.Count, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(3, searchEffects[0].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenTwoDuplicateSearchEffect_ShouldSearchFor_SearchEffectsCount2()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 2},
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 3},
            };

            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            ActionResult result = await _searchEffectsController.SearchEssentialOil(searchEffects);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, searchEffects.Count);
            Assert.AreEqual(searchEffects.Count, searchEffects.Count);
            Assert.AreEqual(searchEffects, searchEffects);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEffects[0].SearchEffectText);
            Assert.AreEqual(1, searchEffects[0].DiscomfortValue);
            Assert.AreEqual("Wirkung gegen Narben", searchEffects[1].SearchEffectText);
            Assert.AreEqual(3, searchEffects[1].DiscomfortValue);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_Given11SearchEssentialOilItemsResultsAndFilterAmountNull_ShouldReturn_EssentialOilsCount11()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = CreateSearchEssentialOilItemsResults();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            PartialViewResult result = await _searchEffectsController.SearchEssentialOil(searchEffects) as PartialViewResult;
            SearchResultViewModel resultModel = result.Model as SearchResultViewModel;

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.AreEqual(11, resultModel.SearchEssentialOilResults.Count);
            Assert.AreEqual(11, resultModel.SearchEssentialOilResultsAmount);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_Given11SearchEssentialOilItemsResults_ShouldReturn_EssentialOilsCount11()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem {SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 1},
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = CreateSearchEssentialOilItemsResults();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            PartialViewResult result = await _searchEffectsController.SearchEssentialOil(searchEffects) as PartialViewResult;
            SearchResultViewModel resultModel = result.Model as SearchResultViewModel;

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.AreEqual(11, resultModel.SearchEssentialOilResults.Count);
            Assert.AreEqual(11, resultModel.SearchEssentialOilResultsAmount);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEssentialOil_GivenSearchEffectTextsInEssentialOil_ShouldReturn_StringOfSearchEffects()
        {
            // Arrange
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem(),
                new SearchEffectItem()
            };
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = CreateSearchEssentialOilItemsResults();
            searchEssentialOilItemsResults[0].SearchEffectTextsInEssentialOil ="Wirkung gegen Schmerzen;@Wirkung gegen Narben";
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            PartialViewResult result = await _searchEffectsController.SearchEssentialOil(searchEffects) as PartialViewResult;
            SearchResultViewModel resultModel = result.Model as SearchResultViewModel;

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.AreEqual("Wirkung gegen Schmerzen;@Wirkung gegen Narben", resultModel.SearchEssentialOilResults[0].SearchEffectTextsInEssentialOil);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task EssentialOilDetails_GivenEssentialOil_ShouldShowView_EssentialOilDetails()
        {
            // Arrange
            string essentialOilId = "idString";
            List<SearchEffectItem> searchEffects = new List<SearchEffectItem>();

            EssentialOil essentialOil = new EssentialOil();
            _essentialOilServiceMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(essentialOil));

            // Act
            PartialViewResult result = await _searchEffectsController.EssentialOilDetails(searchEffects, essentialOilId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Views/SearchEssentialOil/_EssentialOilDetails.cshtml", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetEffectNames_GivenEffectss_ShouldReturnArrayOfNamesOf_Effects()
        {
            // Arrange
            IList<Effect> effectList = new List<Effect> { new Effect { Name = "TestEffect" }, new Effect { Name = "NewEffect" }, new Effect { Name = "some value" }, };
            _effectServiceMock.Setup(e => e.GetAllAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            // Act
            JsonResult result = await _searchEffectsController.GetEffectNames();
            string[] arrayResult = result.Data as string[];
            
            // Assert
            Assert.IsNotNull(arrayResult);
            Assert.AreEqual(3, arrayResult.Length);
            Assert.AreEqual(arrayResult[0], "TestEffect");
            Assert.AreEqual(arrayResult[1], "NewEffect");
            Assert.AreEqual(arrayResult[2], "some value");
            _effectServiceMock.Verify(e => e.GetAllAsync(It.IsAny<EffectFilter>()), Times.Once);
        }

        [Test]
        public async Task SearchEffects_GivenSearchResultList_ShouldShowPartialView_EffectsSearchResults()
        {
            // Arrange
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            PartialViewResult result = await _searchEffectsController.SearchEssentialOil(new List<SearchEffectItem>()) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Views/SearchEffects/_EffectsSearchResults.cshtml", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        [Test]
        public async Task SearchEffects_GivenSearchResultList_ShouldShowPartialView_EffectsSearchResults1()
        {
            // Arrange
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>();
            _essentialOilServiceMock.Setup(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>())).Returns(Task.FromResult(searchEssentialOilItemsResults));

            // Act
            PartialViewResult result = await _searchEffectsController.SearchEssentialOil(new List<SearchEffectItem>()) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Views/SearchEffects/_EffectsSearchResults.cshtml", result.ViewName);
            _essentialOilServiceMock.Verify(e => e.GetEssentialOilResultsBySearchedEffectsNameAsync(It.IsAny<IList<SearchEffectItem>>()), Times.Once);
        }

        public IList<SearchEssentialOilItem> CreateSearchEssentialOilItemsResults()
        {
            IList<SearchEssentialOilItem> searchEssentialOilItemsResults = new List<SearchEssentialOilItem>
            {
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 1"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 2"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 3"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 4"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 5"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 6"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 7"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 8"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 9"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 10"} },
                new SearchEssentialOilItem { EssentialOil = new EssentialOil { Name = "Oil 11"} },
            };

            return searchEssentialOilItemsResults;
        }
    }
}
