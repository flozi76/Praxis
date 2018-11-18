using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using Duftfinder.Database.Repositories;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moq;
using NUnit.Framework;
using Duftfinder.Resources;

namespace Tests.Duftfinder.Database
{
    [TestFixture]
    public class RoleRepositoryTest
    {
        private IRoleRepository _roleRepository;

        // Gets test database from App.config of testing project.
        private readonly MongoContext _dbContext = new MongoContext();

        [SetUp]
        public void SetUp()
        {
            _roleRepository = new RoleRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(Role));
        }

        [Test]
        public async Task GetAllAsync_GivenRoles_ShouldReturn_AllRolesAscending_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<Role> result = await _roleRepository.GetAllAsync(new RoleFilter());

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("a role", result[0].Name);
            Assert.AreEqual("Another role", result[1].Name);
            Assert.AreEqual("my role", result[2].Name);
            Assert.AreEqual("someRole", result[3].Name);
        }
        
        [Test]
        public async Task GetAllAsync_GivenRolesWithSortOrderDescending_ShouldReturn_AllRolesDescending_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            RoleFilter filter = new RoleFilter { SortValues = new Dictionary<string, string> { { "Name", Constants.Descending } } };

            // Act
            IList<Role> result = await _roleRepository.GetAllAsync(filter);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("a role", result[3].Name);
            Assert.AreEqual("Another role", result[2].Name);
            Assert.AreEqual("my role", result[1].Name);
            Assert.AreEqual("someRole", result[0].Name);
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithMatchingName_ShouldGet_RoleThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            RoleFilter filter = new RoleFilter { Name = "a role" };

            // Act
            IList<Role> result = await _roleRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Name == "a role"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenNameFilterWithNoMatchingName_ShouldGet_NoRole_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            RoleFilter filter = new RoleFilter { Name = "a" };

            // Act
            IList<Role> result = await _roleRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _roleRepository.InsertAsync(new Role { Name = "someRole" });
            await _roleRepository.InsertAsync(new Role { Name = "a role" });
            await _roleRepository.InsertAsync(new Role { Name = "my role" });
            await _roleRepository.InsertAsync(new Role { Name = "Another role" });
        }
    }
}