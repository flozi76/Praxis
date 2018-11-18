using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Interfaces.Services;
using NUnit.Framework;
using Moq;

using Duftfinder.Business.Services;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;
using Duftfinder.Resources;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class SmtpEmailServiceTest
    {
        private ISmtpEmailService _smtpEmailService;
        private Mock<IConfigurationService> _configurationServiceMock;

        [SetUp]
        public void SetUp()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();

            _smtpEmailService = new SmtpEmailService(_configurationServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task SendEmailsAsync_GivenEmail_ShouldSendTheEmail()
        {
            // Arrange
            Email emailToSend = GetEmailToSend();

            // Mock multiple calls of same method.
            _configurationServiceMock.SetupSequence(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("smtp.gmail.com"))
                .Returns(Task.FromResult("587"))
                .Returns(Task.FromResult("true"))
                .Returns(Task.FromResult("duftfinder@gmail.com"))
                .Returns(Task.FromResult("MtDuftfinder?2018"))
                .Returns(Task.FromResult("duftfinder@gmail.com"));

            // Act
            await _smtpEmailService.SendEmailAsync(emailToSend, false);

            // Assert
            _configurationServiceMock.Verify(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()), Times.Exactly(6));
        }

        [Test]
        public async Task SendEmailsAsync_GivenEmailWithWrongConfiguration_ShouldContain_ValidationResultError()
        {
            // Arrange
            Email emailToSend = GetEmailToSend();

            //  Mock multiple calls of same method.
            _configurationServiceMock.SetupSequence(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("smtp.gugus.com"))
                .Returns(Task.FromResult("587"))
                .Returns(Task.FromResult("true"))
                .Returns(Task.FromResult("duftfinder@gmail.com"))
                .Returns(Task.FromResult("MtDuftfinder?2018"))
                .Returns(Task.FromResult("duftfinder@gmail.com"));

            // Act
            ValidationResultList validationResult = await _smtpEmailService.SendEmailAsync(emailToSend, false);

            // Assert
            Assert.IsTrue(validationResult.HasErrors);
            string errorValue = $"{Resources.Error_SendingEmailFailed} {Resources.Error_ContactAdmin}";
            Assert.IsTrue(validationResult.Errors.TryGetValue(Constants.SendEmailValidationError, out errorValue));
            _configurationServiceMock.Verify(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()), Times.Exactly(6));
        }

        [Test]
        public async Task SendEmailsAsync_GivenEmailWithWrongConfigurationIsValidationErrorVisibleForAdmin_ShouldContain_ValidationResultErrorWithOutContactAdmin()
        {
            // Arrange
            Email emailToSend = GetEmailToSend();

            //  Mock multiple calls of same method.
            _configurationServiceMock.SetupSequence(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("smtp.gugus.com"))
                .Returns(Task.FromResult("587"))
                .Returns(Task.FromResult("true"))
                .Returns(Task.FromResult("duftfinder@gmail.com"))
                .Returns(Task.FromResult("MtDuftfinder?2018"))
                .Returns(Task.FromResult("duftfinder@gmail.com"));

            // Act
            ValidationResultList validationResult = await _smtpEmailService.SendEmailAsync(emailToSend, true);

            // Assert
            Assert.IsTrue(validationResult.HasErrors);
            string errorValue = $"{Resources.Error_SendingEmailFailed}";
            Assert.IsTrue(validationResult.Errors.TryGetValue(Constants.SendEmailValidationError, out errorValue));
            _configurationServiceMock.Verify(s => s.GetConfigurationParameterByKeyAsync(It.IsAny<string>()), Times.Exactly(6));
        }

        private static Email GetEmailToSend()
        {
            return new Email { EmailAddress = "frogli317@gmail.com", Subject = "Some Subject", EmailHtmlText = "<!DOCTYPE html><html><body><h1>Hello</h1><p>World</p></body></html>"  };
        }
    }
}
