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
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class EssentialOilServiceTest
    {
        private IEssentialOilService _essentialOilService;

        private Mock<IEssentialOilRepository> _essentialOilRepositoryMock;
        private Mock<IEssentialOilMoleculeRepository> _essentialOilMoleculeRepositoryMock;
        private Mock<IEssentialOilEffectRepository> _essentialOilEffectRepositoryMock;
        private Mock<IMoleculeRepository> _moleculeRepositoryMock;
        private Mock<IEffectRepository> _effectRepositoryMock;

        private IList<EssentialOil> _essentialOilList;

        [SetUp]
        public void SetUp()
        {
            _essentialOilRepositoryMock = new Mock<IEssentialOilRepository>();
            _essentialOilMoleculeRepositoryMock = new Mock<IEssentialOilMoleculeRepository>();
            _essentialOilEffectRepositoryMock = new Mock<IEssentialOilEffectRepository>();
            _moleculeRepositoryMock = new Mock<IMoleculeRepository>();
            _effectRepositoryMock = new Mock<IEffectRepository>();

            _essentialOilService = new EssentialOilService(_essentialOilRepositoryMock.Object, _essentialOilEffectRepositoryMock.Object, _essentialOilMoleculeRepositoryMock.Object, _effectRepositoryMock.Object, _moleculeRepositoryMock.Object);

            // Set up data
            _essentialOilList = CreateEssentialOils();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectTextIsWhitespace_ShouldReturn_ListOfSearchEssentialOilItemResults_Count0()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = " ", DiscomfortValue = 3 }
            };

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(0, searchEssentialOilItemResults.Count);
            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Never);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectDiscomfortValue0_ShouldReturn_ListOfSearchEssentialOilItemResults_Count0()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 0 },
                new SearchEffectItem()
            };

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(0, searchEssentialOilItemResults.Count);
            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Never);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectTextIsEmptyDiscomfortValue0_ShouldReturn_ListOfSearchEssentialOilItemResults_Count0()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "", DiscomfortValue = 0 },
                new SearchEffectItem()
            };

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(0, searchEssentialOilItemResults.Count);
            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Never);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenNoSearchedEffectFound_ShouldReturn_ListOfSearchEssentialOilItemResults_Count0()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung", DiscomfortValue = 3 },
                new SearchEffectItem()
            };

            IList<Effect> effectList = new List<Effect>();
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(0, searchEssentialOilItemResults.Count);
            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Never);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffect_WirkungGegenSchmerzenDiscomfortValue3_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 3 },
                new SearchEffectItem()
            };

            IList<Effect> effectList = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46"} };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            IList<EssentialOilEffect> essentialOilEffectList =  new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(6, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[0].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(3, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[2].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[2].SearchEffectTextsInEssentialOil);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectWithWhitespace_WirkungGegenSchmerzenDiscomfortValue1_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen ", DiscomfortValue = 1 },
                new SearchEffectItem()
            };

            IList<Effect> effectList = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46"} };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            IList<EssentialOilEffect> essentialOilEffectList =  new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(2, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[0].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(1, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[1].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(4, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[2].SearchEffectTextsInEssentialOil);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectWithWhitespace_WirkungGegenSchmerzenDiscomfortValue3_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen ", DiscomfortValue = 3 },
                new SearchEffectItem()
            };

            IList<Effect> effectList = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46"} };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            IList<EssentialOilEffect> essentialOilEffectList =  new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(6, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(3, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffect_WirkungGegenSchmerzenDiscomfortValue4_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 4 },
                new SearchEffectItem()
            };

            IList<Effect> effectList = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46"} };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EffectFilter>())).Returns(Task.FromResult(effectList));

            IList<EssentialOilEffect> essentialOilEffectList =  new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(8, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(4, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(16, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Once);
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffect_WirkungGegenSchmerzenDiscomfortValue3_And_WirkungGegenNarbenDiscomfortValue4_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 3 },
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 4 },
                new SearchEffectItem()
            };

            IList<Effect> effectList1 = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Schmerzen"))).Returns(Task.FromResult(effectList1));

            IList<EssentialOilEffect> essentialOilEffectList1 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f46"))).Returns(Task.FromResult(essentialOilEffectList1));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));


            IList<Effect> effectList2 = new List<Effect> { new Effect { Name = "Wirkung gegen Narben", Id = "5a339b56f36d281276999f48" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Narben"))).Returns(Task.FromResult(effectList2));

            IList<EssentialOilEffect> essentialOilEffectList2 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Narben
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 1 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 4 }, // Öl 2
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f48"))).Returns(Task.FromResult(essentialOilEffectList2));

            EssentialOil essentialOil4 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil4));

            EssentialOil essentialOil5 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil5));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(10, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen;@Wirkung gegen Narben", searchEssentialOilItemResults[0].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(19, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen;@Wirkung gegen Narben", searchEssentialOilItemResults[1].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[2].SearchEffectTextsInEssentialOil);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Exactly(2));
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Exactly(2));
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(5));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffectWithWhitespace_WirkungGegenSchmerzenDiscomfortValue3_And_WirkungGegenNarbenDiscomfortValue4_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen ", DiscomfortValue = 3 },
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 4 },
                new SearchEffectItem()
            };

            IList<Effect> effectList1 = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Schmerzen "))).Returns(Task.FromResult(effectList1));

            IList<EssentialOilEffect> essentialOilEffectList1 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f46"))).Returns(Task.FromResult(essentialOilEffectList1));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));


            IList<Effect> effectList2 = new List<Effect> { new Effect { Name = "Wirkung gegen Narben", Id = "5a339b56f36d281276999f48" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Narben"))).Returns(Task.FromResult(effectList2));

            IList<EssentialOilEffect> essentialOilEffectList2 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Narben
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 1 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 4 }, // Öl 2
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f48"))).Returns(Task.FromResult(essentialOilEffectList2));

            EssentialOil essentialOil4 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil4));

            EssentialOil essentialOil5 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil5));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(10, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen;@Wirkung gegen Narben", searchEssentialOilItemResults[0].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(19, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen;@Wirkung gegen Narben", searchEssentialOilItemResults[1].SearchEffectTextsInEssentialOil);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);
            Assert.AreEqual("Wirkung gegen Schmerzen", searchEssentialOilItemResults[2].SearchEffectTextsInEssentialOil);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Exactly(2));
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Exactly(2));
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(5));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffect_WirkungGegenSchmerzenDiscomfortValue3_And_WirkungGegenNarbenDiscomfortValue4_And_WirkungFurDieBlutzirkulationDiscomfortValue2_ShouldReturn_ListOfSearchEssentialOilItemResults_Count4()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 3 },
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 4 },
                new SearchEffectItem { SearchEffectText = "Wirkung für die Blutzirkulation", DiscomfortValue = 4 },
                new SearchEffectItem()
            };

            IList<Effect> effectList1 = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Schmerzen"))).Returns(Task.FromResult(effectList1));

            IList<EssentialOilEffect> essentialOilEffectList1 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f46"))).Returns(Task.FromResult(essentialOilEffectList1));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            IList<Effect> effectList2 = new List<Effect> { new Effect { Name = "Wirkung gegen Narben", Id = "5a339b56f36d281276999f48" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Narben"))).Returns(Task.FromResult(effectList2));

            IList<EssentialOilEffect> essentialOilEffectList2 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Narben
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 1 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 4 }, // Öl 2
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f48"))).Returns(Task.FromResult(essentialOilEffectList2));

            EssentialOil essentialOil4 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil4));

            EssentialOil essentialOil5 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil5));

            
            IList<Effect> effectList3 = new List<Effect> { new Effect { Name = "Wirkung für die Blutzirkulation", Id = "5a339b56f36d281276999f49" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung für die Blutzirkulation"))).Returns(Task.FromResult(effectList3));

            IList<EssentialOilEffect> essentialOilEffectList3 = new List<EssentialOilEffect>
            {
                // Wirkung für die Blutzirkulation
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f49", EssentialOilId = "5a339b56f36d281276999f44", EffectDegree = 2 }, // Öl 3
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f49"))).Returns(Task.FromResult(essentialOilEffectList3));

            EssentialOil essentialOil6 = new EssentialOil { Id = "5a339b56f36d281276999f44", Name = "Öl 3" }; // Öl 3
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f44")).Returns(Task.FromResult(essentialOil6));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(4, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(10, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(19, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);
            Assert.AreEqual("Öl 3", searchEssentialOilItemResults[3].EssentialOil.Name);
            Assert.AreEqual(8, searchEssentialOilItemResults[3].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[3].MatchAmount);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Exactly(3));
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Exactly(3));
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(6));
        }

        [Test]
        public async Task GetEssentialOilResultsBySearchedEffectsNameAsync_GivenSearchedEffect_WirkungGegenSchmerzenDiscomfortValue3_And_WirkungGegenNarbenDiscomfortValue4_And_WirkungGegenMuskelkater1_ShouldReturn_ListOfSearchEssentialOilItemResults_Count3()
        {
            // Arrange
            IList<SearchEffectItem> searchedEffects = new List<SearchEffectItem>
            {
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Schmerzen", DiscomfortValue = 3 },
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Narben", DiscomfortValue = 4 },
                new SearchEffectItem { SearchEffectText = "Wirkung gegen Muskelkater", DiscomfortValue = 1 },
                new SearchEffectItem()
            };

            IList<Effect> effectList1 = new List<Effect> { new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Schmerzen"))).Returns(Task.FromResult(effectList1));

            IList<EssentialOilEffect> essentialOilEffectList1 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f46"))).Returns(Task.FromResult(essentialOilEffectList1));

            EssentialOil essentialOil1 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil1));

            EssentialOil essentialOil2 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil2));

            EssentialOil essentialOil3 = new EssentialOil { Id = "5a339b56f36d281276999f45", Name = "Öl 4" }; // Öl 4
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f45")).Returns(Task.FromResult(essentialOil3));

            IList<Effect> effectList2 = new List<Effect> { new Effect { Name = "Wirkung gegen Narben", Id = "5a339b56f36d281276999f48" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Narben"))).Returns(Task.FromResult(effectList2));

            IList<EssentialOilEffect> essentialOilEffectList2 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Narben
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 1 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 4 }, // Öl 2
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f48"))).Returns(Task.FromResult(essentialOilEffectList2));

            EssentialOil essentialOil4 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil4));

            EssentialOil essentialOil5 = new EssentialOil { Id = "5a339b56f36d281276999f43", Name = "Öl 2" }; // Öl 2
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f43")).Returns(Task.FromResult(essentialOil5));

            
            IList<Effect> effectList3 = new List<Effect> { new Effect { Name = "Wirkung gegen Muskelkater", Id = "5a339b56f36d281276999f47" } };
            _effectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EffectFilter>(f => f.Name == "Wirkung gegen Muskelkater"))).Returns(Task.FromResult(effectList3));

            IList<EssentialOilEffect> essentialOilEffectList3 = new List<EssentialOilEffect>
            {
                // Wirkung gegen Muskelkater
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f47", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 3 }, // Öl 1
            };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.Is<EssentialOilEffectFilter>(f => f.EffectId == "5a339b56f36d281276999f47"))).Returns(Task.FromResult(essentialOilEffectList3));

            EssentialOil essentialOil6 = new EssentialOil { Id = "5a339b56f36d281276999f42", Name = "Öl 1" }; // Öl 1
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync("5a339b56f36d281276999f42")).Returns(Task.FromResult(essentialOil6));

            // Act 
            IList<SearchEssentialOilItem> searchEssentialOilItemResults = await _essentialOilService.GetEssentialOilResultsBySearchedEffectsNameAsync(searchedEffects);

            // Assert
            Assert.AreEqual(3, searchEssentialOilItemResults.Count);
            Assert.AreEqual("Öl 1", searchEssentialOilItemResults[0].EssentialOil.Name);
            Assert.AreEqual(13, searchEssentialOilItemResults[0].EffectDegreeDiscomfortValue);
            Assert.AreEqual(3, searchEssentialOilItemResults[0].MatchAmount);
            Assert.AreEqual("Öl 2", searchEssentialOilItemResults[1].EssentialOil.Name);
            Assert.AreEqual(19, searchEssentialOilItemResults[1].EffectDegreeDiscomfortValue);
            Assert.AreEqual(2, searchEssentialOilItemResults[1].MatchAmount);
            Assert.AreEqual("Öl 4", searchEssentialOilItemResults[2].EssentialOil.Name);
            Assert.AreEqual(12, searchEssentialOilItemResults[2].EffectDegreeDiscomfortValue);
            Assert.AreEqual(1, searchEssentialOilItemResults[2].MatchAmount);

            _effectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EffectFilter>()), Times.Exactly(3));
            _essentialOilEffectRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>()), Times.Exactly(3));
            _essentialOilRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(6));
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue1AndSliderValue1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 1 = 1
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 1, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1"} };
            // maxEffectDegreeDiscomfortValue 1 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 4;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 1 / 4 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue3AndSliderValue1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 3 = 3
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 3, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 1 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 4;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 3 / 4 * 100 = 75
            Assert.AreEqual(75, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue2AndSliderValue1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 2 = 2
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 2, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 1 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 4;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 2 / 4 * 100  = 50
            Assert.AreEqual(50, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue4AndSliderValue1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 4 = 4
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 4, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 1 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 4;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 4 / 4 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue12AndSliderValue4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 = 12
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 12, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 16;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 12 / 16 * 100 = 75
            Assert.AreEqual(75, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue16AndSliderValue4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 = 16
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 16, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 = 4
            int maxEffectDegreeDiscomfortValue = 16;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 16 / 16 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount2AndEffectDegreeDiscomfortValue6AndSliderValue4And2_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 1 = 6
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 6, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 = 24
            int maxEffectDegreeDiscomfortValue = 24;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 8 / 24 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount2AndEffectDegreeDiscomfortValue12AndSliderValue4And2_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 4 = 12
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 12, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 = 24
            int maxEffectDegreeDiscomfortValue = 24;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 12 / 24 * 100 = 50
            Assert.AreEqual(50, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount2AndEffectDegreeDiscomfortValue14AndSliderValue4And2_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 2 and 2 * 3 = 14
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 14, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 = 24
            int maxEffectDegreeDiscomfortValue = 24;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 14 / 24 * 100 = 58,333 -> 58
            Assert.AreEqual(58, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue7AndSliderValue4And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 1 and 1 * 1 = 7
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 7, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 1 * 4 = 28
            int maxEffectDegreeDiscomfortValue = 28;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 7 / 28 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue12AndSliderValue4And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 3 and 1 * 2 = 12
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 12, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 1 * 4 = 28
            int maxEffectDegreeDiscomfortValue = 28;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 12 / 28 * 100 = 42,857 -> 43
            Assert.AreEqual(43, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue19AndSliderValue4And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 2 * 1 and 1 * 1 = 19
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 19, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 1 * 4 = 28
            int maxEffectDegreeDiscomfortValue = 28;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 19 / 28 * 100 = 67,857 -> 68
            Assert.AreEqual(68, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue18AndSliderValue4And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 and 2 * 2 and 1 * 2 = 18
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 18, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 1 * 4 = 28
            int maxEffectDegreeDiscomfortValue = 28;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 18 / 28 * 100 = 64,286 -> 64
            Assert.AreEqual(64, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue28AndSliderValue4And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 2 * 4 and 1 * 4 = 28
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 28, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 1 * 4 = 28
            int maxEffectDegreeDiscomfortValue = 28;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 28 / 28 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount4AndEffectDegreeDiscomfortValue9AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 1 and 2 * 1 and 1 * 1 = 9
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 9, MatchAmount = 4, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 9 / 36 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount4AndEffectDegreeDiscomfortValue16AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 2 * 3 and 2 * 2 and 1 * 2 = 16
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 16, MatchAmount = 4, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 16 / 36 * 100 = 44,444 -> 44
            Assert.AreEqual(44, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount4AndEffectDegreeDiscomfortValue28AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 and 2 * 3 and 2 * 4 and 1 * 2 = 28
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 28, MatchAmount = 4, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);


            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 28 / 36 * 100 = 77,777 -> 78
            Assert.AreEqual(78, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue22AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 and 2 * 4 and 1 * 2 = 22
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 22, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);


            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 22 / 36 * 100 = 61,111 -> 61
            Assert.AreEqual(61, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount2AndEffectDegreeDiscomfortValue14AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 and 1 * 2 = 14
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 14, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);


            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 14 / 36 * 100 = 38,889 -> 39
            Assert.AreEqual(39, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue2AndSliderValue4And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. and 1 * 2 = 2
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 2, MatchAmount = 1, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4  and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 36;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);


            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 2 / 36 * 100 = 5,556 -> 6
            Assert.AreEqual(6, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue12AndSliderValue4And3And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 3 * 1 and 2 * 1 and 2 * 1 and 1 * 1 = 12
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 12, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 48;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 36 / 48 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue23AndSliderValue4And3And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 3 * 2 and 2 * 1 and 2 * 4 and 1 * 3 = 23
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 23, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 48;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 23 / 48 * 100 = 47,917 -> 48
            Assert.AreEqual(48, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue29AndSliderValue4And3And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 1 and 3 * 4 and 2 * 1 and 2 * 4 and 1 * 3 = 29
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 29, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 48;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 29 / 48 * 100 = 60,417
            Assert.AreEqual(60, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue37AndSliderValue4And3And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 3 and 3 * 4 and 2 * 1 and 2 * 4 and 1 * 3 = 37
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 37, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 48;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 37 / 48 * 100 = 77,083 -> 77
            Assert.AreEqual(77, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue48AndSliderValue4And3And2And2And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 48
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 48, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 3 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            int maxEffectDegreeDiscomfortValue = 48;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 48 / 48 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue80AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 80, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 80 / 80 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue20AndSliderValue1And1And1And1And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 = 20
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 20, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 = 20
            int maxEffectDegreeDiscomfortValue = 20;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 20 / 20 * 100 = 100
            Assert.AreEqual(100, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount5AndEffectDegreeDiscomfortValue5AndSliderValue1And1And1And1And1_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 1 and 1 * 1 and 1 * 1 and 1 * 1 and 1 * 1 = 5
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 5, MatchAmount = 5, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 and 1 * 4 = 20
            int maxEffectDegreeDiscomfortValue = 20;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 5 / 20 * 100 = 25
            Assert.AreEqual(25, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount4AndEffectDegreeDiscomfortValue36AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 2 * 4 and 2 * 4 and 1 * 4 = 36
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 36, MatchAmount = 4, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 36 / 80 * 100 = 45
            Assert.AreEqual(45, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount3AndEffectDegreeDiscomfortValue28AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 2 * 4 and 1 * 4 = 28
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 28, MatchAmount = 3, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 28 / 80 * 100 = 35
            Assert.AreEqual(35, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount2AndEffectDegreeDiscomfortValue24AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 and 2 * 4 = 24
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 24, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 24 / 80 * 100 = 30
            Assert.AreEqual(30, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue4AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 1 * 4 = 4
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 4, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 4 / 80 * 100 = 5
            Assert.AreEqual(5, weightedMatchValue);
        }

        [Test]
        public void CalculateWeightedMatchValue_GivenEssentialOilWithMatchAmount1AndEffectDegreeDiscomfortValue16AndSliderValue4And4And4And4And4_ShouldReturn_CorrectWeightedMatchValue()
        {
            // Arrange
            // EffectDegreeDiscomfortValue e.g. 4 * 4 = 16
            SearchEssentialOilItem resultItem = new SearchEssentialOilItem { EffectDegreeDiscomfortValue = 4, MatchAmount = 2, EssentialOil = new EssentialOil { Name = "Öl 1" } };
            // maxEffectDegreeDiscomfortValue 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 and 4 * 4 = 80
            int maxEffectDegreeDiscomfortValue = 80;

            // Act
            int weightedMatchValue = _essentialOilService.CalculateWeightedMatchValue(resultItem, maxEffectDegreeDiscomfortValue);

            // Assert
            // EffectDegreeDiscomfortValue / maxEffectDegreeDiscomfortValue * 100
            // 4 / 80 * 100 = 5
            Assert.AreEqual(5, weightedMatchValue);
        }



        [Test]
        public async Task GetAssignedEffectsForEssentialOilAsync_GivenNoEffectAssigned_ShouldReturn_ListOfEffect_Count0()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect>();
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));

            // Act 
            IList<Effect> result = await _essentialOilService.GetAssignedEffectsForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(0, result.Count);
            _effectRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetAssignedEffectsForEssentialOilAsync_GivenOneEffectAssigned_ShouldReturn_ListOfEffect_Count1()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect { EffectDegree = 4} };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));
            Effect effect = new Effect();
            _effectRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(effect));

            // Act 
            IList<Effect> result = await _essentialOilService.GetAssignedEffectsForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(4, result[0].EffectDegree);
            _effectRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetAssignedEffectsForEssentialOilAsync_GivenTwoEffectsAssigned_ShouldReturn_ListOfEffect_Count2()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilEffect> essentialOilEffectList = new List<EssentialOilEffect> { new EssentialOilEffect { EffectDegree = 4 }, new EssentialOilEffect { EffectDegree = 2 } };
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffectList));
            Effect effect = new Effect();
            _effectRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(effect));

            // Act 
            IList<Effect> result = await _essentialOilService.GetAssignedEffectsForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(2, result.Count);
            _effectRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetAssignedMoleculesForEssentialOilAsync_GivenTwoMoleculesAssigned_ShouldReturn_ListOfEffect_Count2()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule> { new EssentialOilMolecule { MoleculePercentage = 29 }, new EssentialOilMolecule { MoleculePercentage = 5 } };
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));
            Molecule molecule = new Molecule();
            _moleculeRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(molecule));

            // Act 
            IList<Molecule> result = await _essentialOilService.GetAssignedMoleculesForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(2, result.Count);
            _moleculeRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetAssignedMoleculesForEssentialOilAsync_GivenNoMoleculesAssigned_ShouldReturn_ListOfEffect_Count0()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule>();
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));

            // Act 
            IList<Molecule> result = await _essentialOilService.GetAssignedMoleculesForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(0, result.Count);
            _moleculeRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetAssignedMoleculesForEssentialOilAsync_GivenOneMoleculesAssigned_ShouldReturn_ListOfEffect_Count1()
        {
            // Arrange
            string essentialOilId = "5a339b56f36d281276999f42";
            IList<EssentialOilMolecule> essentialOilMoleculeList = new List<EssentialOilMolecule> { new EssentialOilMolecule { MoleculePercentage = 29 } };
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMoleculeList));
            Molecule molecule = new Molecule();
            _moleculeRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(molecule));

            // Act 
            IList<Molecule> result = await _essentialOilService.GetAssignedMoleculesForEssentialOilAsync(essentialOilId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(29, result[0].MoleculePercentage);
            _moleculeRepositoryMock.Verify(e => e.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetAllAsync_GivenListOfEssentialOils_ShouldReturn_ListOfEssentialOils()
        {
            // Arrange
            _essentialOilRepositoryMock.Setup(e => e.GetAllAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(_essentialOilList));
            EssentialOilFilter filter = new EssentialOilFilter();

            // Act 
            Task<IList<EssentialOil>> result = _essentialOilService.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(3, result.Result.Count);
            Assert.AreEqual(_essentialOilList, result.Result);
        }

        [Test]
        public void GetByIdAsync_GivenIdOfEssentialOil_ShouldReturn_EssentialOil()
        {
            // Arrange
            string id = "idString";
            EssentialOil essentialOil = new EssentialOil { Name = "NewEssentialOil" };
            _essentialOilRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(essentialOil));

            // Act 
            Task<EssentialOil> result = _essentialOilService.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(essentialOil, result.Result);
        }

        [Test]
        public void InsertAsync_GivenNewEssentialOil_ShouldInsert_NewEssentialOil()
        {
            // Arange 
            EssentialOil essentialOil = new EssentialOil { Name = "NewEssentialOil" };

            // Act 
            Task<ValidationResultList> validationResult = _essentialOilService.InsertAsync(essentialOil);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.InsertAsync(It.IsAny<EssentialOil>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void UpdateAsync_GivenNewEssentialOil_ShouldUpdate_NewEssentialOil()
        {
            // Arange 
            EssentialOil essentialOil = new EssentialOil { Name = "NewEssentialOil" };

            // Act 
            Task<ValidationResultList> validationResult = _essentialOilService.UpdateAsync(essentialOil);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.UpdateAsync(It.IsAny<EssentialOil>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenEssentialOilId_ShouldDelete_EssentialOil()
        {
            // Arange 
            string id = "idString";

            // Act 
            Task<ValidationResultList> validationResult = _essentialOilService.DeleteAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsNull(validationResult.Result);
        }

        [Test]
        public void DeleteAsync_GivenEssentialOilIdIsNull_ShouldNotDelete_EssentialOil()
        {
            // Arange 
            string id = null;

            // Act 
            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Throws(new ArgumentNullException());

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteEssentialOilWithAssignmentsAsync_GivenEssentialOilIdWithNoAssignments_ShouldDelete_EssentialOil()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule>();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect>();

            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));

            // Act 
            validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEssentialOilWithAssignmentsAsync_GivenEssentialOilIdWithMoleculesAndEffectAssigned_ShouldDelete_EssentialOilAndAssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule(), new EssentialOilMolecule() };
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect() };

            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Exactly(2));
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEssentialOilWithAssignmentsAsync_GivenEssentialOilIdWithMoleculeAssigned_ShouldDelete_EssentialOilAndAssignedMolecule()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule() };
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect>();

            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEssentialOilWithAssignmentsAsync_GivenEssentialOilIdWithEffectAssigned_ShouldDelete_EssentialOilAndAssignedEffect()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList();
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule>();
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect() };

            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _essentialOilEffectRepositoryMock.Setup(e => e.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act 
            validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(validationResult.HasErrors);
        }

        [Test]
        public async Task DeleteEssentialOilWithAssignmentsAsync_GivenDeleteEssentialOilWithValidationResultError_ShouldNotDelete_AssignedValues()
        {
            // Arange 
            string id = "idString";
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            IList<EssentialOilMolecule> essentialOilMolecules = new List<EssentialOilMolecule> { new EssentialOilMolecule(), new EssentialOilMolecule() };
            IList<EssentialOilEffect> essentialOilEffects = new List<EssentialOilEffect> { new EssentialOilEffect() };

            _essentialOilRepositoryMock.Setup(e => e.DeleteAsync(id)).Returns(Task.FromResult(validationResult));
            _essentialOilMoleculeRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilMoleculeFilter>())).Returns(Task.FromResult(essentialOilMolecules));
            _essentialOilEffectRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilEffectFilter>())).Returns(Task.FromResult(essentialOilEffects));

            // Act 
            validationResult = await _essentialOilService.DeleteEssentialOilWithAssignmentsAsync(id);

            // Assert
            _essentialOilRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Once);
            _essentialOilMoleculeRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            _essentialOilEffectRepositoryMock.Verify(e => e.DeleteAsync(It.IsAny<string>()), Times.Never);
            Assert.IsTrue(validationResult.HasErrors);
            Assert.IsTrue(validationResult.Errors.ContainsKey("errorKey"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenDefaultFilter_ShouldReturn_ListOfEssentialOils()
        {
            // Arrange 
            _essentialOilRepositoryMock.Setup(e => e.GetByFilterAsync(It.IsAny<EssentialOilFilter>())).Returns(Task.FromResult(_essentialOilList));
;
            // Act
            IList<EssentialOil> result = await _essentialOilService.GetByFilterAsync(new EssentialOilFilter());

            // Assert
            Assert.AreEqual(3, result.Count);
            _essentialOilRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<EssentialOilFilter>()), Times.Once);
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

        // Values are not used, but give an overview for the Search Effect tests.
        private void SetupDataForSearchTests()
        {
            List<EssentialOil> essentialOilListForEffectSearchTests = new List<EssentialOil>
            {
                new EssentialOil { Name = "Öl 1", Id = "5a339b56f36d281276999f42"},
                new EssentialOil { Name = "Öl 2", Id = "5a339b56f36d281276999f43"},
                new EssentialOil { Name = "Öl 3", Id = "5a339b56f36d281276999f44"},
                new EssentialOil { Name = "Öl 4", Id = "5a339b56f36d281276999f45"},
            };

            List<Effect> effectListForEffectSearchTests = new List<Effect>
            {
                new Effect { Name = "Wirkung gegen Schmerzen", Id = "5a339b56f36d281276999f46"},
                new Effect { Name = "Wirkung gegen Muskelkater", Id = "5a339b56f36d281276999f47"},
                new Effect { Name = "Wirkung gegen Narben", Id = "5a339b56f36d281276999f48"},
                new Effect { Name = "Wirkung für die Blutzirkulation", Id = "5a339b56f36d281276999f49"},
            };

            List<EssentialOilEffect> essentialOilEffectListForEffectSearchTests = new List<EssentialOilEffect>
            {
                // Wirkung gegen Schmerzen
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 2 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 1 }, // Öl 2
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f46", EssentialOilId = "5a339b56f36d281276999f45", EffectDegree = 4 }, // Öl 4

                // Wirkung gegen Muskelkater
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f47", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 3 }, // Öl 1

                // Wirkung gegen Narben
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f42", EffectDegree = 1 }, // Öl 1
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f48", EssentialOilId = "5a339b56f36d281276999f43", EffectDegree = 4 }, // Öl 2

                // Wirkung für die Blutzirkulation
                new EssentialOilEffect {  EffectId = "5a339b56f36d281276999f49", EssentialOilId = "5a339b56f36d281276999f44", EffectDegree = 2 }, // Öl 3
            };
        }
    }
}
