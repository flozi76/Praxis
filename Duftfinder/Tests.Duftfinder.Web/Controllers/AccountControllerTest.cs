using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using NUnit.Framework;
using Duftfinder.Web.Controllers;
using Moq;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Resources;
using Duftfinder.Web.Helpers;
using Duftfinder.Web.Models;

namespace Tests.Duftfinder.Web.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        private AccountController _accountController;

        private Mock<IUserService> _userServiceMock;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        
        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();

            _accountController = new AccountController(_userServiceMock.Object, _authenticationServiceMock.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Login_GivenModelStateIsInValid_ShouldShowView_Login()
        {
            // Arrange
            _accountController.ModelState.AddModelError("errorKey", "new error message");
            LoginViewModel loginViewModel = new LoginViewModel();

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Never);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNoEmail_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User>();
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            LoginViewModel loginViewModel = new LoginViewModel { Password = "somepwhash" };

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual(Resources.Error_EmailOrPasswordInvalid, errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNoPassword_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "admin@email.com", PasswordHash = "somepwhash", RoleIdString = "5a339b56f36d281276999f42" } };
            Role role = new Role { Name = RoleValue.Admin.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Failure;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.Is<string>(s => s == null))).Returns(Task.FromResult(signInStatus));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "admin@email.com" };

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual(Resources.Error_EmailOrPasswordInvalid, errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNoValidEmail_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User>();
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            LoginViewModel loginViewModel = new LoginViewModel {Email = "anyemail@gmail.com", Password = "somepwhash"};

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual(Resources.Error_EmailOrPasswordInvalid, errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNoValidPassword_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "admin@email.com", PasswordHash = "somepwhash", RoleIdString = "5a339b56f36d281276999f42" } };
            Role role = new Role { Name = RoleValue.Admin.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Failure;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.Is<string>(s => s == "wrongpwhash"))).Returns(Task.FromResult(signInStatus));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "admin@email.com", Password = "wrongpwhash" };

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual(Resources.Error_EmailOrPasswordInvalid, errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNotConfirmedUser_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "notconfirmed@email.com", PasswordHash = "somepwhash", RoleIdString = "5a339b56f36d281276999f43" } };
            Role role = new Role { Name = RoleValue.Friend.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "notconfirmed@email.com", Password = "somepwhash" };

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual(Resources.Error_EmailNotYetConfirmed, errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenInactiveUser_ShouldShowView_Login()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "inactive@email.com", PasswordHash = "somepwhash", IsConfirmed = true, IsInactive = true, RoleIdString = "5a339b56f36d281276999f43" } };
            Role role = new Role { Name = RoleValue.Friend.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "inactive@email.com", Password = "somepwhash" };

            // Act
            ViewResult result = await _accountController.Login(loginViewModel, null) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual($@"{Resources.Error_AccountInactive} {Resources.Error_ContactAdmin}", errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Login_GivenNoRole_ShouldRedirectToAction_SearchEssentialOilIndexAndSignIn()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "norole@email.com", PasswordHash = "somepwhash", IsConfirmed = true } };
            Role role = new Role();
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Success;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(signInStatus));
            _authenticationServiceMock.Setup(a => a.SignIn(It.IsAny<string>()));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "norole@email.com", Password = "somepwhash" };

            // Mock Url.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _accountController.Login(loginViewModel, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("SearchEssentialOil", result.RouteValues["controller"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Login_GivenRoleAdmin_ShouldRedirectToAction_SearchEssentialOilIndexAndSignIn()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "admin@email.com", PasswordHash = "somepwhash", RoleIdString = "5a339b56f36d281276999f42" } };
            Role role = new Role { Name = RoleValue.Admin.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Success;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(signInStatus));
            _authenticationServiceMock.Setup(a => a.SignIn(It.IsAny<string>()));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "admin@email.com", Password = "somepwhash" };

            // Mock Url.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _accountController.Login(loginViewModel, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("SearchEssentialOil", result.RouteValues["controller"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Login_GivenConfirmedUser_ShouldRedirectToAction_SearchEssentialOilIndexAndSignIn()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "confirmed@email.com", PasswordHash = "somepwhash", IsConfirmed = true, RoleIdString = "5a339b56f36d281276999f43" } };
            Role role = new Role { Name = RoleValue.Friend.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Success;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(signInStatus));
            _authenticationServiceMock.Setup(a => a.SignIn(It.IsAny<string>()));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "confirmed@email.com", Password = "somepwhash" };

            // Mock Url.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _accountController.Login(loginViewModel, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("SearchEssentialOil", result.RouteValues["controller"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Login_GivenConfirmedUserAndSearchEffectsUrl_ShouldRedirectTo_SearchEffectsAndSignIn()
        {
            // Arrange
            IList<User> userList = new List<User> { new User { Email = "confirmed@email.com", PasswordHash = "somepwhash", IsConfirmed = true, RoleIdString = "5a339b56f36d281276999f43" } };
            Role role = new Role { Name = RoleValue.Friend.ToString() };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));
            SignInStatus signInStatus = SignInStatus.Success;
            _userServiceMock.Setup(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(signInStatus));
            _authenticationServiceMock.Setup(a => a.SignIn(It.IsAny<string>()));
            LoginViewModel loginViewModel = new LoginViewModel { Email = "confirmed@email.com", Password = "somepwhash" };

            // Mock Url.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
            urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectResult result = await _accountController.Login(loginViewModel, "/Duftfinder.Web/searcheffects") as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/Duftfinder.Web/searcheffects", result.Url);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.SignInWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _authenticationServiceMock.Verify(a => a.SignIn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Logoff_ShouldRedirectToAction_SearchEssentialOilIndexAndSignOut()
        {
            // Act
            RedirectToRouteResult result = _accountController.Logoff() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("SearchEssentialOil", result.RouteValues["controller"]);
            _authenticationServiceMock.Verify(x => x.SignOut(), Times.Once());
        }

        [Test]
        public async Task Register_GivenModelStateIsInvalid_ShouldShowView_Register()
        {
            // Arrange
            _accountController.ModelState.AddModelError("errorKey", "new error message");
            RegisterViewModel registerViewModel = new RegisterViewModel();

            // Act
            ViewResult result = await _accountController.Register(registerViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("Register", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Never);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Register_GivenUserAlreadyRegistered_ShouldShowView_Register()
        {
            // Arrange
            RegisterViewModel registerViewModel = new RegisterViewModel();

            IList<User> userList = new List<User> { new User { Email = "notconfirmed@email.com", PasswordHash = "somepwhash" } };
            _userServiceMock.Setup(x => x.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            
            // Act
            ViewResult result = await _accountController.Register(registerViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual($@"{Resources.Error_RegistrationFailed} {Resources.Error_UserAllreadyRegistered}", errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("Register", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Register_GivenGenerateVerifyAccountKeyIsNull_ShouldShowView_Register()
        {
            // Arrange
            string id = "5a339b56f36d281276999f42";
            RegisterViewModel registerViewModel = new RegisterViewModel();
            IList<User> userList = new List<User>();
            string email = "notconfirmed@email.com";
            IList<User> registeredUserList = new List<User> { new User { Id = id, Email = email, PasswordHash = "somepwhash", VerifyAccountKey = null } };
            ValidationResultList validationResultList = new ValidationResultList();
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(validationResultList));

            // Mock multiple calls of same method.
            _userServiceMock.SetupSequence(x => x.GetByFilterAsync(It.IsAny<UserFilter>()))
                .Returns(Task.FromResult(userList))
                .Returns(Task.FromResult(registeredUserList));

            // Act
            ViewResult result = await _accountController.Register(registerViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Exactly(2));
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Register_GivenRegisteredUser_ShouldRedirectToAction_RegistrationConfirmation_And_SendEmailToUser()
        {
            // Arrange
            RegisterViewModel registerViewModel = new RegisterViewModel();
            IList<User> userList = new List<User>();
            string email = "notconfirmed@email.com";
            Guid verifyAccountKey = Guid.NewGuid();
            IList<User> registeredUserList = new List<User> { new User { Id = "5a339b56f36d281276999f42", Email = email, PasswordHash = "somepwhash", VerifyAccountKey = verifyAccountKey} };

            ValidationResultList validationResultList = new ValidationResultList();
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(validationResultList));
            _userServiceMock.Setup(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validationResultList));

            // Mock multiple calls of same method.
            _userServiceMock.SetupSequence(x => x.GetByFilterAsync(It.IsAny<UserFilter>()))
                .Returns(Task.FromResult(userList))
                .Returns(Task.FromResult(registeredUserList));

            // Mock Url.Action.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x. Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("http://xxx/Account/VerifyAccount?email=" + email + "&verifyAccountKey=" + verifyAccountKey);
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _accountController.Register(registerViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("RegistrationConfirmation", result.RouteValues["action"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Exactly(2));
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_GivenModelStateIsInValid_ShouldShowView_ChangePassword()
        {
            // Arrange
            _accountController.ModelState.AddModelError("errorKey", "new error message");
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();

            // Act
            ViewResult result = await _accountController.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePassword", result.ViewName);
            _userServiceMock.Verify(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public async Task ChangePassword_GivenEmailValidationResultError_ShouldContain_EmailModelStateError()
        {
            // Arrange
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();

            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>(Constants.Email, "new error message") } };
            _userServiceMock.Setup(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _accountController.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("Email", _accountController.ModelState.Keys.FirstOrDefault());
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePassword", result.ViewName);
            _userServiceMock.Verify(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_GivenPasswordValidationResultError_ShouldContain_OldPasswordModelStateError()
        {
            // Arrange
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();

            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>(Constants.Password, "new error message") } };
            _userServiceMock.Setup(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _accountController.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("OldPassword", _accountController.ModelState.Keys.FirstOrDefault());
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePassword", result.ViewName);
            _userServiceMock.Verify(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_GivenPasswordChanged_ShouldRedirectToAction_ChangePasswordConfirmation()
        {
            // Arrange
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
            _userServiceMock.Setup(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _accountController.ChangePassword(changePasswordViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeOrResetPasswordConfirmation", result.RouteValues["action"]);
            _userServiceMock.Verify(u => u.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ForgotPassword_GivenModelStateIsInvalid_ShouldShowView_ForgotPassword()
        {
            // Arrange
            _accountController.ModelState.AddModelError("errorKey", "new error message");
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();

            // Act
            ViewResult result = await _accountController.ForgotPassword(forgotPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPassword", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Never);
            _userServiceMock.Verify(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Test]
        public async Task ForgotPassword_GivenNoUserExists_ShouldShowView_ForgotPassword()
        {
            // Arrange
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();

            IList<User> userList = new List<User>();
            _userServiceMock.Setup(x => x.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));

            // Act
            ViewResult result = await _accountController.ForgotPassword(forgotPasswordViewModel) as ViewResult;

            // Assert
            Assert.IsFalse(_accountController.ModelState.IsValid);
            string errorMessage = _accountController.ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage;
            Assert.AreEqual($@"{Resources.Error_NoUserWithEmail}", errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPassword", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ForgotPassword_GivenGeneratePasswordResetKeyIsNull_ShouldShowView_ForgotPassword()
        {
            // Arrange
            string id = "5a339b56f36d281276999f42";
            string email = "email";
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel { Email = email};
            IList<User> userList = new List<User> { new User { Id =  id, Email = email } };
            IList<User> userListWithResetKey = new List<User> { new User { Id = id, Email = email } };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Mock multiple calls of same method.
            _userServiceMock.SetupSequence(x => x.GetByFilterAsync(It.IsAny<UserFilter>()))
                .Returns(Task.FromResult(userList))
                .Returns(Task.FromResult(userListWithResetKey));

            // Act
            ViewResult result = await _accountController.ForgotPassword(forgotPasswordViewModel) as ViewResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("ForgotPassword", result.ViewName);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Exactly(2));
            _userServiceMock.Verify(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ForgotPassword_GivenGeneratePasswordResetKeyIsValid_ShouldRedirectToAction_ForgotPasswordConfirmation_And_SendEmailToUser()
        {
            // Arrange
            string id = "5a339b56f36d281276999f42";
            string email = "email";
            Guid passwordResetKey = Guid.NewGuid();
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel { Email = email};
            IList<User> userList = new List<User> { new User { Id =  id, Email = email } };
            IList<User> userListWithResetKey = new List<User> { new User { Id = id, Email = email, PasswordResetKey = passwordResetKey } };
            _userServiceMock.Setup(u => u.GetByFilterAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(userList));
            _userServiceMock.Setup(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _userServiceMock.Setup(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Mock multiple calls of same method.
            _userServiceMock.SetupSequence(x => x.GetByFilterAsync(It.IsAny<UserFilter>()))
                .Returns(Task.FromResult(userList))
                .Returns(Task.FromResult(userListWithResetKey));

            // Mock Url.Action.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("http://xxx/Account/ResetPassword?email=" + email + "&verifyAccountKey=" + passwordResetKey);
            _accountController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _accountController.ForgotPassword(forgotPasswordViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPasswordConfirmation", result.RouteValues["action"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.GetByFilterAsync(It.IsAny<UserFilter>()), Times.Exactly(2));
            _userServiceMock.Verify(u => u.GenerateAndUpdatePasswordResetKeyAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ResetPassword_GivenModelStateIsInvalid_ShouldShowView_ResetPassword()
        {
            // Arrange
            _accountController.ModelState.AddModelError("errorKey", "new error message");
            User user = new User { Email = "email", PasswordHash = "somepwhash", PasswordResetKey = Guid.NewGuid() };
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel { Email = user.Email, PasswordResetKey = (Guid)user.PasswordResetKey };

            // Act
            ViewResult result = await _accountController.ResetPassword(resetPasswordViewModel) as ViewResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("ResetPassword", result.ViewName);
            _userServiceMock.Verify(u => u.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ResetPassword_GivenValidationResultError_ShouldShowView_ResetPassword()
        {
            // Arrange
            User user = new User { Email = "email", PasswordHash = "somepwhash", PasswordResetKey = Guid.NewGuid() };

            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel { Email = user.Email, PasswordResetKey = (Guid)user.PasswordResetKey };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>(Constants.Email, "new error message") } };
            _userServiceMock.Setup(u => u.ResetPasswordAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<Guid>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _accountController.ResetPassword(resetPasswordViewModel) as ViewResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_accountController.ModelState.IsValid);
            Assert.AreEqual("ResetPassword", result.ViewName);
            _userServiceMock.Verify(u => u.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task ResetPassword_GivenPasswordResetKey_ShouldRedirectToAction_ChangeOrResetPasswordConfirmation()
        {
            // Arrange
            User user = new User { Email = "email", PasswordHash = "somepwhash", PasswordResetKey = Guid.NewGuid() };
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel { Email = user.Email, PasswordResetKey = (Guid)user.PasswordResetKey };
            _userServiceMock.Setup(u => u.ResetPasswordAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<Guid>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _accountController.ResetPassword(resetPasswordViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeOrResetPasswordConfirmation", result.RouteValues["action"]);
            Assert.IsTrue(_accountController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}
