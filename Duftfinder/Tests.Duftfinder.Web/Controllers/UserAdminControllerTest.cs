using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Duftfinder.Domain.Entities;
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
    public class UserAdminControllerTest
    {
        private UserAdminController _userAdminController;

        private Mock<IUserService> _userServiceMock;

        private IList<User> _userList;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();

            _userAdminController = new UserAdminController(_userServiceMock.Object);

            // Set up data
            _userList = CreateUserList();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        [Test]
        public async Task Index_GivenListOfUsers_ShouldReturn_ListOfUsers()
        {
            // Arrange
            _userServiceMock.Setup(u => u.GetAllAsync(It.IsAny<UserFilter>())).Returns(Task.FromResult(_userList));
            Role role = new Role();
            _userServiceMock.Setup(u => u.GetRoleForUserAsync(It.IsAny<string>())).Returns(Task.FromResult(role));

            // Act 
            ViewResult result = await _userAdminController.Index() as ViewResult;
            IList<UserViewModel> resultModel = result.Model as IList<UserViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultModel);
            Assert.AreEqual(5, resultModel.Count);
            _userServiceMock.Verify(u => u.GetRoleForUserAsync(It.IsAny<string>()), Times.Exactly(4)); // Not called for user "norole@email.com".
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateUser_ShouldRedirectToAction_Index()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewUserWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            // Act
            ViewResult result = await _userAdminController.CreateOrEdit(userViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewUserWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(validationResult));


            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsFalse(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewUserWithPasswordValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("Password", "new error message") } };
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(validationResult));


            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsFalse(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenCreateNewUserWithPasswordModelStateError_ShouldContain_ModelStateError()
        {
            // Arrange
            _userAdminController.ModelState.AddModelError("Password", "new error message");

            UserViewModel userViewModel = new UserViewModel { Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));


            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsFalse(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.RegisterAccountAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUser_ShouldRedirectToAction_Index()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User user = new User { PasswordHash = "somepwhash "};
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithValidationResultError_ShouldShowView_CreateOrEdit()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));

            User user = new User { PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            ViewResult result = await _userAdminController.CreateOrEdit(userViewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CreateOrEdit", result.ViewName);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithValidationResultError_ShouldContain_ModelStateError()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));

            User user = new User { PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsFalse(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithPasswordValidationResultError_ShouldNotContain_ModelStateError()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("Password", "new error message") } };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(validationResult));

            User user = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsFalse(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithPasswordAndPassword2ModelStateError_ShouldRedirectToAction_Index()
        {
            // Arrange
            _userAdminController.ModelState.AddModelError("Password", "new error message");
            _userAdminController.ModelState.AddModelError("Password2", "new error message");

            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User user = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithPasswordModelStateError_ShouldNotContain_ModelStateError()
        {
            // Arrange
            _userAdminController.ModelState.AddModelError("Password", "new error message");

            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User user = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithPassword2ModelStateError_ShouldNotContain_ModelStateError()
        {
            // Arrange
            _userAdminController.ModelState.AddModelError("Password2", "new error message");

            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User user = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserWithPasswordAndPassword2ModelStateError_ShouldNotContain_ModelStateError()
        {
            // Arrange
            _userAdminController.ModelState.AddModelError("Password", "new error message");
            _userAdminController.ModelState.AddModelError("Password2", "new error message");

            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com" };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User user = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash " };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            await _userAdminController.CreateOrEdit(userViewModel);

            // Assert
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserIsUserConfirmation_ShouldRedirectToAction_Index_And_SendEmailToUser()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com", IsConfirmed = true };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User existingUser = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash", IsConfirmed = false };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(existingUser));

            _userServiceMock.Setup(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));

            // Mock Url.Action.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("http://xxx/Account/Login?email=" + userViewModel.Email);
            _userAdminController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserIsNotUserConfirmationTrueFalse_ShouldRedirectToAction_Index_And_NotSendEmailToUser()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com", IsConfirmed = false };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User existingUser = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash", IsConfirmed = true };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(existingUser));

            // Mock Url.Action.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("http://xxx/Account/Login?email=" + userViewModel.Email);
            _userAdminController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateOrEdit_GivenEditUserIsNotUserConfirmationTrueTrue_ShouldRedirectToAction_Index_And_NotSendEmailToUser()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42", Email = "notconfirmed@email.com", IsConfirmed = true };
            _userServiceMock.Setup(u => u.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(new ValidationResultList()));

            User existingUser = new User { Id = "5a339b56f36d281276999f42", PasswordHash = "somepwhash", IsConfirmed = true };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(existingUser));

            // Mock Url.Action.
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("http://xxx/Account/Login?email=" + userViewModel.Email);
            _userAdminController.Url = urlHelperMock.Object;

            // Act
            RedirectToRouteResult result = await _userAdminController.CreateOrEdit(userViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsTrue(_userAdminController.ModelState.IsValid);
            _userServiceMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GenerateAndSendMailForUserAsync(It.IsAny<User>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ShowConfirmDelete_GivenLoggedInUser_ShouldReturn_NotificationDialog()
        {
            // Arrange
            FakeHttpContext("admin@email.com");
            string id = "idString";
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()));

            // Act
            PartialViewResult result = await _userAdminController.ShowConfirmDelete(id, "admin@email.com") as PartialViewResult;
            ConfirmationViewModel resultModel = result.Model as ConfirmationViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Resources.Notification_DeleteUserNotPossible_Text, resultModel.DialogText);
            Assert.IsNull(resultModel.Action);
            Assert.AreEqual("~/Views/Shared/_Notification.cshtml", result.ViewName);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShowConfirmDelete_GivenSystemAdminUser_ShouldReturn_NotificationDialog()
        {
            // Arrange
            FakeHttpContext("admin@email.com");
            string id = "idString";
            User user = new User() {IsSystemAdmin = true};
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            PartialViewResult result = await _userAdminController.ShowConfirmDelete(id, "admin@email.com") as PartialViewResult;
            ConfirmationViewModel resultModel = result.Model as ConfirmationViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Resources.Notification_DeleteUserSystemAdminNotPossible_Text, resultModel.DialogText);
            Assert.IsNull(resultModel.Action);
            Assert.AreEqual("~/Views/Shared/_Notification.cshtml", result.ViewName);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShowConfirmDelete_GivenNotLoggedInUser_ShouldReturn_ConfirmationDialog()
        {
            // Arrange
            FakeHttpContext("admin@email.com");
            string id = "idString";
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()));


            // Act
            PartialViewResult result = await _userAdminController.ShowConfirmDelete(id, "confirmed@email.com") as PartialViewResult;
            ConfirmationViewModel resultModel = result.Model as ConfirmationViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Resources.Confirmation_Delete_Text, resultModel.DialogText);
            Assert.AreEqual("UserAdmin/Delete", resultModel.Action);
            Assert.AreEqual("~/Views/Shared/_Confirmation.cshtml", result.ViewName);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteExistingUser_ShouldReturn_EmptyResult()
        {
            // Arrange
            string id = "idString";
            _userServiceMock.Setup(u => u.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(new ValidationResultList()));
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()));

            // Act
            EmptyResult result = await _userAdminController.Delete(id) as EmptyResult;

            // Assert
            Assert.IsNotNull(result);
            _userServiceMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenSystemAdminUser_ShouldReturn_JsonErrorResultWithErrorMessageAndNotDeleteUser()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));

            User user = new User { Email = "admin@email.com", IsSystemAdmin = true };
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            // Act
            JsonErrorResult result = await _userAdminController.Delete(userViewModel.Id) as JsonErrorResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{ errorMessage = " + string.Format(Resources.Notification_DeleteUserSystemAdminNotPossible_Text, user.Email)+ " }", result.Data.ToString());
            _userServiceMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_GivenDeleteUserWithValidationResultError_ShouldReturn_JsonErrorResultWithErrorMessage()
        {
            // Arrange
            UserViewModel userViewModel = new UserViewModel { Id = "5a339b56f36d281276999f42" };
            ValidationResultList validationResult = new ValidationResultList { Errors = { new KeyValuePair<string, string>("errorKey", "new error message") } };
            _userServiceMock.Setup(u => u.DeleteAsync(It.IsAny<string>())).Returns(Task.FromResult(validationResult));
            _userServiceMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()));

            // Act
            JsonErrorResult result = await _userAdminController.Delete(userViewModel.Id) as JsonErrorResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("{ errorMessage = new error message }", result.Data.ToString());
            _userServiceMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Once);
            _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Fakes HttpContext.User.Identity.Name;
        /// </summary>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="loggedInEmail"></param>
        private void FakeHttpContext(string loggedInEmail)
        {
            Mock<HttpContextBase> fakeHttpContext = new Mock<HttpContextBase>();
            GenericIdentity fakeIdentity = new GenericIdentity(loggedInEmail);
            GenericPrincipal principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);

            _userAdminController.ControllerContext = new ControllerContext();
            _userAdminController.ControllerContext.HttpContext = fakeHttpContext.Object;
        }

        private IList<User> CreateUserList()
        {
            IList<User> userList = new List<User>
            {
                new User { Email = "admin@email.com", PasswordHash = "somepwhash", RoleIdString = "5a339b56f36d281276999f42" },
                new User { Email = "confirmed@email.com", PasswordHash = "somepwhash", IsConfirmed  = true, RoleIdString = "5a339b56f36d281276999f43"},
                new User { Email = "notconfirmed@email.com", PasswordHash = "somepwhash", IsConfirmed  = false, RoleIdString = "5a339b56f36d281276999f43"},
                new User { Email = "inactive@email.com", PasswordHash = "somepwhash", IsConfirmed  = true, IsInactive = true, RoleIdString = "5a339b56f36d281276999f43"},
                new User { Email = "norole@email.com", PasswordHash = "somepwhash", IsConfirmed  = true },
            };

            return userList;
        }
    }
}
