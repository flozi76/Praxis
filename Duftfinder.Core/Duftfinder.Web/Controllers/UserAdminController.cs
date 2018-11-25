using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Helpers;
using Duftfinder.Web.Models;
using log4net;


namespace Duftfinder.Web.Controllers
{
    /// <summary>
    /// The Controller for the "Benutzerverwaltung" stuff in the Adminbereich.
    /// "Ätherisches Öl suchen" is accessible for everyone.
    /// <author>Anna Krebs</author>
    /// </summary>
    [Authorize(Roles = Constants.Admin)]
    public class UserAdminController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IUserService _userService;

        public UserAdminController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ActionResult> Index()
        {
            UserFilter filter = new UserFilter();

            // Get all users from database.
            IList<User> users = await _userService.GetAllAsync(filter);
            IList<UserViewModel> userViewModels = new List<UserViewModel>();

            // Create list of users for view. 
            foreach (User user in users)
            {
                UserViewModel model = new UserViewModel(user, null);

                if (user.RoleIdString != null)
                {
                    Role role = await _userService.GetRoleForUserAsync(user.RoleIdString);

                    // Set name of role for user.
                    model.RoleValueString = role.Name;
                }

                userViewModels.Add(model);
            }

            return View(userViewModels);
        }

        /// <summary>
        /// Shows view for create or edit for user.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(string id)
        {
            IList<Role> roles = await _userService.GetAllRolesAsync(new RoleFilter());

            UserViewModel model;

            // Get UserViewModel according to whether is edit or create.
            if (!string.IsNullOrEmpty(id))
            {
                // Edit
                User user = await _userService.GetByIdAsync(id);

                if (user == null)
                {
                    Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
                    throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
                }

                model = new UserViewModel(user, roles);
            }
            else
            {
                // Create
                model = new UserViewModel(null, roles);
            }

            return View(model);
        }

        /// <summary>
        /// Creates or edits a user after save was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(UserViewModel model)
        {
            ValidationResultList validationResult = new ValidationResultList();

            User user = new User();

            // Map view model to entity.
            model.MapViewModelToEntity(user);

            RemovePasswordRequiredForEdit(user.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    // Edit or create
                    if (user.Id != null)
                    {
                        // Edit

                        // Don't override password, if password is null.
                        User existingUser = await _userService.GetByIdAsync(user.Id);
                        if (model.Password == null)
                        {
                            user.PasswordHash = existingUser.PasswordHash;
                        }

                        validationResult = await PrepareAndSendConfirmUserEmail(existingUser, user);

                        // Only update if user email doesn't already exist.
                        ValidationResultList validationResultUpdate = await _userService.UpdateAsync(user);

                        if (validationResultUpdate.HasErrors)
                        {
                            validationResult.Errors.Add(Constants.UpdateUserValidationError, validationResultUpdate.Errors.Values.FirstOrDefault());
                        }
                    }
                    else
                    {
                        // Create
                        // Only insert if user email doesn't already exist.
                        validationResult = await _userService.RegisterAccountAsync(user, model.Password);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"An unexpected error occurred while inserting or editing: {e}");
                    throw new ArgumentException(Resources.Resources.Error_UnexpectedError);
                }
            }

            // Show validation result, if validation error occurred while 
            // inserting or if ModelState is invalid.
            if (validationResult.HasErrors || !ModelState.IsValid)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);

                // Set substances to display in drop down.
                IList<Role> roles = await _userService.GetAllRolesAsync(new RoleFilter());
                model.Roles = roles;

                Log.Info("Show CreateOrEdit");
                return View(nameof(CreateOrEdit), model);
            }

            Log.Info("Redirect to Index");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Shows delete confirmation or notification after delete was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <param name="name">IMPORTANT: has to be "name" in order to be called from Dialog.js.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ShowConfirmDelete(string id, string name)
        {
            string emailToDelete = name;
            ConfirmationViewModel model = new ConfirmationViewModel {Id = id, Name = name};

            // Check if logged in user is System Admin.
            User user = await _userService.GetByIdAsync(id);
            if (user.IsSystemAdmin)
            {
                // User is System Admin. Show notification that not possible to delete System Admin.
                model.DialogTitle = Resources.Resources.Notification_DeleteNotPossible_Title;
                model.DialogText = Resources.Resources.Notification_DeleteUserSystemAdminNotPossible_Text;

                return PartialView("~/Views/Shared/_Notification.cshtml", model);
            }

            // Show notification or confirmation according to whether user wants to delete himself or other entry.
            string loggedInEmail = HttpContext.User.Identity.Name;

            // Check if logged in user deletes himself.
            if (loggedInEmail == emailToDelete)
            {
                // User wants to delete himself. Show notification that not possible.
                model.DialogTitle = Resources.Resources.Notification_DeleteNotPossible_Title;
                model.DialogText = Resources.Resources.Notification_DeleteUserNotPossible_Text;

                return PartialView("~/Views/Shared/_Notification.cshtml", model);
            }
            else
            {
                // Show confirmation.
                model.DialogTitle = Resources.Resources.Confirmation_Delete_Title;
                model.DialogText = Resources.Resources.Confirmation_Delete_Text;
                model.Action = Constants.UserAdminDelete;

                return PartialView("~/Views/Shared/_Confirmation.cshtml", model);
            }
        }

        /// <summary>
        /// Shows notification after checkbox isConfirmed was checked & when isAccountVerified is false.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <param name="name">IMPORTANT: has to be "name" in order to be called from Dialog.js.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowNotifyAccountNotVerified(string id, string name)
        {
            string notVerifiedEmail = name;
            ConfirmationViewModel model = new ConfirmationViewModel
            {
                Id = id,
                Name = notVerifiedEmail,
                DialogTitle = Resources.Resources.Notification_AccountNotVerified_Title,
                DialogText = $"{Resources.Resources.Notification_AccountNotVerified_Text} {Resources.Resources.Notification_EmailWillBeSentToUser_Text}"
            };

            return PartialView("~/Views/Shared/_Notification.cshtml", model);
        }

        /// <summary>
        /// Shows notification after checkbox isConfirmed was checked to notify the admin, that email will be sent to user after save.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <param name="name">IMPORTANT: has to be "name" in order to be called from Dialog.js.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowNotifyEmailWillBeSentToUser(string id, string name)
        {
            string notVerifiedEmail = name;
            ConfirmationViewModel model = new ConfirmationViewModel
            {
                Id = id,
                Name = notVerifiedEmail,
                DialogTitle = Resources.Resources.Notification_EmailWillBeSentToUser_Title,
                DialogText = Resources.Resources.Notification_EmailWillBeSentToUser_Text
            };

            return PartialView("~/Views/Shared/_Notification.cshtml", model);
        }

        /// <summary>
        /// Deletes an user after delete was clicked.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                ValidationResultList validationResult = new ValidationResultList();

                // Check if logged in user is System Admin.
                User user = await _userService.GetByIdAsync(id);
                if (!user.IsSystemAdmin)
                {
                    // Delete.
                    validationResult = await _userService.DeleteAsync(id);
                }
                else
                {
                    // Can't delete System Admin.
                    validationResult.Errors.Add(string.Empty, string.Format(Resources.Resources.Notification_DeleteUserSystemAdminNotPossible_Text, user.Email));
                }

                // Show validation result, if error occurred.
                if (validationResult.HasErrors)
                {
                    Log.Error($"User with id {id} could not be deleted");
                    return new JsonErrorResult($"{validationResult.Errors.Values.SingleOrDefault()}");
                }
            }
            catch (Exception e)
            {
                // Show general error message if exception occurred.
                Log.Error($"An unexpected error occurred while deleting: {e}");
                return new JsonErrorResult($"{Resources.Resources.Error_UnexpectedError}");
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Prepares and sends the email, that is sent to the new registered user in order to verify his account.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="existingUser"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> PrepareAndSendConfirmUserEmail(User existingUser, User user)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // User is changed from not confirmed to confirmed by admin.
            bool isUserConfirmation = !existingUser.IsConfirmed && user.IsConfirmed;

            // Send email to user, if admin confirmed the user.
            if (isUserConfirmation)
            {
                // Get url from user mail.
                string loginUrl = Url.Action("Login", "Account", new { email = user.Email });
                string loginUrlFull = Url.Action("Login", "Account", new { email = user.Email }, Request?.Url?.Scheme) ?? loginUrl;
                Log.Info($"loginUrlFull is {loginUrlFull}");

                if (loginUrlFull != null)
                {
                    // Prepare the email & send the email to the user.
                    validationResult = await _userService.GenerateAndSendMailForUserAsync(user, new Uri(loginUrlFull), Constants.InfoAboutRegistrationConfirmationSubject, Constants.InfoAboutRegistrationConfirmationText);
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Removes password required validation from ModelState for edit.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="userId"></param>
        private void RemovePasswordRequiredForEdit(string userId)
        {
            if (userId != null)
            {
                if (ModelState.ContainsKey(Constants.Password))
                {
                    ModelState[Constants.Password].Errors.Clear();
                }

                if (ModelState.ContainsKey(Constants.Password2))
                {
                    ModelState[Constants.Password2].Errors.Clear();
                }
            }
        }

        /// <summary>
        /// Add validation results errors to ModelState in order to show in validation summary on view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="errors"></param>
        private void AddValidationResultsToModelStateErrors(IDictionary<string, string> errors)
        {
            foreach (KeyValuePair<string, string> error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Value);
            }
        }
    }
}