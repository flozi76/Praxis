using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Interfaces.Services;
using NUnit.Framework;
using Moq;

using Duftfinder.Business.Services;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;
using Duftfinder.Resources;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class EmailServiceTest
    {
        private IEmailService _emailService;
        private Mock<IConfigurationService> _configurationServiceMock;

        [SetUp]
        public void SetUp()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();

            _emailService = new EmailService(_configurationServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task GenerateEmailForUser_GivenUserAndVerifyBackUri_ShouldGenerateTheEmail()
        {
            // Arrange
            string email = "frogli317@gmail.com";
            User user = new User { Email = email};
            Guid verifyAccountKey = Guid.NewGuid();
            string verifyAccountUrlFull = "http://localhost:8080/Account/VerifyAccount?email=" + email + "&verifyAccountKey=" + verifyAccountKey;
            Uri verifyBackUri = new Uri(verifyAccountUrlFull);
            string emailSubject = "Some Subject";
            string emailText = "<p>Follow link: {0}</p>";
            
            // Mock multiple calls of same method.
            _configurationServiceMock.SetupSequence(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(emailSubject))
                .Returns(Task.FromResult(emailText));

            // Act
            Email result = await _emailService.GenerateEmailForUser(user, verifyBackUri, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.AreEqual("frogli317@gmail.com", result.EmailAddress);
            Assert.AreEqual("Some Subject", result.Subject);
            Assert.IsTrue(result.EmailHtmlText.Contains(verifyAccountUrlFull));
            Assert.AreEqual("<p>Follow link: http://localhost:8080/Account/VerifyAccount?email=frogli317@gmail.com&verifyAccountKey=" + verifyAccountKey.ToString() +"</p>", result.EmailHtmlText);
            _configurationServiceMock.Verify(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public async Task GenerateInfoEmailForAdmin_GivenUserAndAdminEmail_ShouldGenerateTheEmail()
        {
            // Arrange
            string userEmail = "frogli_baebi@hotmail.com";
            string emailSubject = "Some Subject";
            string emailText = "<p>User with email {0} is verified</p>";
            string adminEmail = "frogli317@gmail.com";

            // Mock multiple calls of same method.
            _configurationServiceMock.SetupSequence(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(emailSubject))
                .Returns(Task.FromResult(emailText))
                .Returns(Task.FromResult(adminEmail));

            // Act
            Email result = await _emailService.GenerateInfoEmailForAdmin(userEmail, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.AreEqual("frogli317@gmail.com", result.EmailAddress);
            Assert.AreEqual("Some Subject", result.Subject);
            Assert.IsTrue(result.EmailHtmlText.Contains(userEmail));
            Assert.AreEqual("<p>User with email " + userEmail + " is verified</p>", result.EmailHtmlText);
            _configurationServiceMock.Verify(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
