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
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Tests.Duftfinder.Business
{
    [TestFixture]
    public class UserServiceTest
    {
        private IUserService _userService;

        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<ICryptoService> _cryptoServiceMock;
        private Mock<ISmtpEmailService> _smtpEmailServiceMock;
        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _cryptoServiceMock = new Mock<ICryptoService>();
            _smtpEmailServiceMock = new Mock<ISmtpEmailService>();
            _emailServiceMock = new Mock<IEmailService>();

            _userService = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _cryptoServiceMock.Object, _emailServiceMock.Object, _smtpEmailServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task GetRoleForUserAsync_GivenRoleId_ShouldReturn_Role()
        {
            // Arrange
            string roleId = "5a339b56f36d281276999f42";
            Role role = new Role();
            _roleRepositoryMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(role));

            // Act
            Role result = await _userService.GetRoleForUserAsync(roleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(role, result);
            _roleRepositoryMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }
                    
        [Test]
        public async Task RegisterAccountAsync_GivenExistingEmail_ShouldCall_InsertAsync_TimesNever()
        {
            // Arrange
            IList<User> userList = new List<User> { new User() };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            ValidationResultList validationResult = new ValidationResultList();
            _userRepositoryMock.Setup(u => u.InsertAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));
            User user = new User {FirstName = "firstName", LastName = "lastName", Email = "mail"};

            // Act
            ValidationResultList result = await _userService.RegisterAccountAsync(user, "password");

            // Assert
            Assert.IsTrue(result.HasErrors);
            _userRepositoryMock.Verify(u => u.InsertAsync(It.IsAny<User>()), Times.Never);
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("password"), Times.Never);
        }

        [Test]
        public async Task RegisterAccountAsync_GivenValidRegistration_ShouldCall_InsertAsync_TimesOnce()
        {
            // Arrange
            IList<User> userList = new List<User>();
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _cryptoServiceMock.Setup(c => c.GeneratePasswordHash(It.IsAny<string>())).Returns("somepwhash");

            ValidationResultList validationResult = new ValidationResultList();
            _userRepositoryMock.Setup(u => u.InsertAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));
            User user = new User { FirstName = "firstName", LastName = "lastName", Email = "mail" };

            // Act
            ValidationResultList result = await _userService.RegisterAccountAsync(user, "password");

            // Assert
            Assert.IsFalse(result.HasErrors);
            _userRepositoryMock.Verify(u => u.InsertAsync(It.IsAny<User>()), Times.Once);
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("password"), Times.Once);
        }

        [Test]
        public async Task SignInWithPasswordAsync_GivenNoEmailAndPasswordExist_ShouldReturn_SignInStatusFailure()
        {
            // Arrange
            IList<User> userList = new List<User>();
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            SignInStatus result = await _userService.SignInWithPasswordAsync("someEmail", "somePassword");

            // Assert
            Assert.AreEqual(SignInStatus.Failure, result);
            _userRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _cryptoServiceMock.Verify(c => c.ValidatePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task SignInWithPasswordAsync_GivenEmailAndWrongPassword_ShouldReturn_SignInStatusFailure()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "someEmail", PasswordHash = "somePasswordHash" } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            _cryptoServiceMock.Setup(c => c.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            SignInStatus result = await _userService.SignInWithPasswordAsync("someEmail", "somePassword");

            // Assert
            Assert.AreEqual(SignInStatus.Failure, result);
            _userRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _cryptoServiceMock.Verify(c => c.ValidatePassword("somePassword", "somePasswordHash"), Times.Once);
        }

        [Test]
        public async Task SignInWithPasswordAsync_GivenEmailAndPasswordValid_ShouldReturn_SignInStatusSuccess()
        {
            // Arrange
            IList<User> userList = new List<User> {new User { Email = "someEmail", PasswordHash = "somePasswordHash" } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            _cryptoServiceMock.Setup(c => c.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            SignInStatus result = await _userService.SignInWithPasswordAsync("someEmail", "somePassword");

            // Assert
            Assert.AreEqual(SignInStatus.Success, result);
            _userRepositoryMock.Verify(e => e.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _cryptoServiceMock.Verify(c => c.ValidatePassword("somePassword", "somePasswordHash"), Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_GivenEmailDoesNotExist_Returns_EmailValidationError()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            IList<User> userList = new List<User>();
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            ValidationResultList result = await _userService.ChangePasswordAsync(email, password, "newPassword");

            // Assert
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("Email", result.Errors.Keys.FirstOrDefault());
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
            _cryptoServiceMock.Verify(c => c.ValidatePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("newPassword"), Times.Never);
        }

        [Test]
        public async Task ChangePasswordAsync_GivenWrongOldPassword_Returns_PasswordValidationError()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            IList<User> userList = new List<User> { new User { Email = email, PasswordHash = "someOtherPassword"} };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            ValidationResultList result = await _userService.ChangePasswordAsync(email, password, "newPassword");

            // Assert
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("Password", result.Errors.Keys.FirstOrDefault());
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
            _cryptoServiceMock.Verify(c => c.ValidatePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("newPassword"), Times.Never);
        }

        [Test]
        public async Task ChangePasswordAsync_GivenPasswordChanged_UpdatesUser()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            string newPasswordHash = "ABC-123";
            IList<User> userList = new List<User> { new User { Email = "someEmail", PasswordHash = "Somepw" } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _cryptoServiceMock.Setup(e => e.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _cryptoServiceMock.Setup(e => e.GeneratePasswordHash(It.IsAny<string>())).Returns(newPasswordHash);

            // Act
            await _userService.ChangePasswordAsync(email, password, "newPassword");

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Email == email && u.PasswordHash == newPasswordHash)), Times.Once);
        }

        [Test]
        public async Task GenerateAndUpdatePasswordResetKeyAsync_GivenEmailDoesNotExist_Returns_EmailValidationError()
        {
            // Arrange
            string email = "someEmail";
            IList<User> userList = new List<User>();
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            ValidationResultList result = await _userService.GenerateAndUpdatePasswordResetKeyAsync(email);

            // Assert
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("Email", result.Errors.Keys.FirstOrDefault());
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task GenerateAndUpdatePasswordResetKeyAsync_GivenEmailExists_ShouldUpdateUser()
        {
            // Arrange
            string email = "someEmail";
            IList<User> userList = new List<User> { new User { Email = email } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            await _userService.GenerateAndUpdatePasswordResetKeyAsync(email);

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Email == email && u.PasswordResetKey != null)), Times.Once());
        }

        [Test]
        public async Task ResetPasswordAsync_GivenEmailDoesNotExist_Returns_EmailValidationError()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            Guid passwordResetKey = Guid.NewGuid();
            IList<User> userList = new List<User>();
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            ValidationResultList result = await _userService.ResetPasswordAsync(email, password, passwordResetKey);

            // Assert
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("Email", result.Errors.Keys.FirstOrDefault());
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("somePassword"), Times.Never);
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ResetPasswordAsyn_GivenPaasswortResetKeyNotMatchEmail_ShouldThrowAnException()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            Guid passwordResetKey = Guid.NewGuid();
            IList<User> userList = new List<User> { new User { Email = email, PasswordResetKey = passwordResetKey } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.ResetPasswordAsync(email, password, Guid.NewGuid()));
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("somePassword"), Times.Never);
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task ResetPasswordAsync_GivenPasswordResetKeyExists_ShouldUpdateUser()
        {
            // Arrange
            string email = "someEmail";
            string password = "somePassword";
            Guid passwordResetKey = Guid.NewGuid();
            IList<User> userList = new List<User> { new User { Email = email, PasswordResetKey = passwordResetKey } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _cryptoServiceMock.Setup(e => e.GeneratePasswordHash(It.IsAny<string>())).Returns(password);

            // Act
            await _userService.ResetPasswordAsync(email, password, passwordResetKey);

            // Assert
            _cryptoServiceMock.Verify(c => c.GeneratePasswordHash("somePassword"), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once());
        }

        [Test]
		public void VerifyAccountAsync_GivenVerifyAccountKeyDoesNotMatchEmail_ShouldThrowAnException()
		{
			// Arrange
			string email = "some@email";
			Guid verifyAccountKey = Guid.NewGuid();
		    IList<User> userList = new List<User> { new User { Email = email, VerifyAccountKey = verifyAccountKey } };
		    _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Assert
		    Assert.ThrowsAsync<InvalidOperationException>(() => _userService.VerifyAccountAsync(email, Guid.NewGuid()));
		}

        [Test]
		public void VerifyAccountAsync_GivenEmailDoesNotExist_ShouldThrowAnException()
		{
            // Arrange
		    string email = "some@email";
            Guid verifyAccountKey = Guid.NewGuid();
		    IList<User> userList = new List<User>();
		    _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Assert
		    Assert.ThrowsAsync<InvalidOperationException>(() => _userService.VerifyAccountAsync(email, verifyAccountKey));
		}

        [Test]
        public async Task VerifyAccountAsync_GivenUpdateAsyncWithValidationResultError_ShouldCallSendEmailNever()
        {
            // Arrange
            string email = "some@email";
            Guid verifyAccountKey = Guid.NewGuid();
            IList<User> userList = new List<User> { new User { Email = email, VerifyAccountKey = verifyAccountKey } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));

            // Act
            await _userService.VerifyAccountAsync(email, verifyAccountKey);

            // Assert
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.Is<User>(x => x.IsAccountVerified && x.VerifyAccountKey == null)), Times.Once);
            _emailServiceMock.Verify(u => u.GenerateInfoEmailForAdmin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _smtpEmailServiceMock.Verify(u => u.SendEmailAsync(It.IsAny<Email>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task VerifyAccountAsync_GivenExistingEmailAndMatchingVerifyAccountKey_ShouldCallUpdateAsyncOnUserRepositoryOnceAndSendEmail()
        {
            // Arrange
            string email = "some@email";
            Guid verifyAccountKey = Guid.NewGuid();
            IList<User> userList = new List<User> { new User { Email = email, VerifyAccountKey = verifyAccountKey } };
            _userRepositoryMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            await _userService.VerifyAccountAsync(email, verifyAccountKey);

            // Assert
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.Is<User>(x => x.IsAccountVerified && x.VerifyAccountKey == null)), Times.Once);
            _emailServiceMock.Verify(u => u.GenerateInfoEmailForAdmin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _smtpEmailServiceMock.Verify(u => u.SendEmailAsync(It.IsAny<Email>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task GenerateAndSendMailForUserAsync_GivenEmail_ShouldGenerateAndSendMail()
        {
            // Arrange
            User user = new User { Email = "someEmail" };
            Uri emailUri = new Uri("http://xxx/Account/Login?email=" + user.Email);
            string emailSubjectConfigurationKey = "someEmailSubjectConfigurationKey";
            string emailTextConfigurationKey = "someEmailTextConfigurationKey";

            // Act
            await _userService.GenerateAndSendMailForUserAsync(user, emailUri, emailSubjectConfigurationKey, emailTextConfigurationKey);

            // Assert
            _emailServiceMock.Verify(u => u.GenerateEmailForUser(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _smtpEmailServiceMock.Verify(u => u.SendEmailAsync(It.IsAny<Email>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
