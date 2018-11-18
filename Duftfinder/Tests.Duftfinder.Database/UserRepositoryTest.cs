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
    public class UserRepositoryTest
    {
        private IUserRepository _userRepository;

        // Gets test database from App.config of testing project.
        private readonly MongoContext _dbContext = new MongoContext();

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(_dbContext);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Delete collection in MongoDB after every test.
            _dbContext.Database.DropCollection(nameof(User));
        }

        [Test]
        public async Task GetAllAsync_GivenUsers_ShouldReturn_AllUsers_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();

            // Act
            IList<User> result = await _userRepository.GetAllAsync(new UserFilter());

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("x@email.com", result[0].Email);
            Assert.AreEqual("a@email.com", result[1].Email);
            Assert.AreEqual("b@email.com", result[2].Email);
            Assert.AreEqual("c@email.com", result[3].Email);
            Assert.AreEqual("z@email.com", result[4].Email);
            Assert.AreEqual("s@email.com", result[5].Email);
        }

        [Test]
        public async Task GetByFilterAsync_GivenEmailFilterWithMatchingName_ShouldGet_UserThatEqualsName_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            UserFilter filter = new UserFilter { Email = "b@email.com" };

            // Act
            IList<User> result = await _userRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(r => r.Email == "b@email.com"));
        }

        [Test]
        public async Task GetByFilterAsync_GivenEmailFilterWithNoMatchingName_ShouldGet_NoUser_FromDatabase()
        {
            // Arrange
            await PrepareDatabase();
            UserFilter filter = new UserFilter { Email = "@email.com" };

            // Act
            IList<User> result = await _userRepository.GetByFilterAsync(filter);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private async Task PrepareDatabase()
        {
            // Inserts values in DB for testing.
            await _userRepository.InsertAsync(new User { Email = "z@email.com", LastName = "aname", FirstName = "bfirstname" });
            await _userRepository.InsertAsync(new User { Email = "s@email.com", LastName = "xname", FirstName = "xfirstname" });
            await _userRepository.InsertAsync(new User { Email = "x@email.com", LastName = "xname", FirstName = "xfirstname", IsSystemAdmin = true });
            await _userRepository.InsertAsync(new User { Email = "c@email.com", LastName = "aname", FirstName = "bfirstname" });
            await _userRepository.InsertAsync(new User { Email = "b@email.com", LastName = "aname", FirstName = "afirstname" });
            await _userRepository.InsertAsync(new User { Email = "a@email.com", LastName = "aname", FirstName = "afirstname" });
        }
    }
}