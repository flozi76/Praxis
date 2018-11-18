using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Interfaces.Services;
using NUnit.Framework;
using Moq;

using Duftfinder.Business.Services;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class CryptoServiceTest
    {
        private ICryptoService _cryptoService;

        [SetUp]
        public void SetUp()
        {
            _cryptoService = new CryptoService();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public void GeneratePasswordHash_GivenPasswordString_ShouldReturnSaltAndPasswordAsString()
        {
            // Arrange
            string password = "password1234$";

            // Act
            string result = _cryptoService.GeneratePasswordHash(password);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [Test]
        public void ValidatePassword_GivenAPasswordAndCorrectHashedPassword_ShouldReturnTrue()
        {
            // Arrange
            string password = "password1234$";
            string hashedPassword = "20000:oxeELB5KY4k/nNB061PbNw==:HHy67v1AI/ILp2sizjCBKkxY9IE=";

            // Act
            bool result = _cryptoService.ValidatePassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidatePassword_GivenAPasswordAndFalseHashedPassword_ShouldReturnFalse()
        {
            // Arrange
            const string password = "password1234$";
            const string hashedPassword = "20000:oxeELB5KY4k/nNB061Pbzz==:HHy67v1AI/ILp2sizjCBKkxY9ZZ=";

            // Act
            bool result = _cryptoService.ValidatePassword(password, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidatePassword_GivenAWrongPasswordAndHashedPassword_ShouldReturnFalse()
        {
            // Arrange
            const string password = "password1234$_asdfasdfasdfasdf";
            const string hashedPassword = "20000:oxeELB5KY4k/nNB061PbNw==:HHy67v1AI/ILp2sizjCBKkxY9IE=";

            // Act
            bool result = _cryptoService.ValidatePassword(password, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
