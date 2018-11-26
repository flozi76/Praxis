using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Web.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Duftfinder.Web.Controllers
{
    /// <summary>
    /// The Controller for all Login, Logout, Register etc. stuff.
    /// <author>Anna Krebs</author>
    /// </summary>
    public class AccountController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IUserService _userService;

        private readonly IAuthenticationService _authenticationService;

        public AccountController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Show Login view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string email)
        {
            LoginViewModel model = new LoginViewModel { Email = email };

            return View(model);
        }

        /// <summary>
        /// Login Duftfinder.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ValidationResultList validationResult = new ValidationResultList();
            
            // Inputted values from view.
            string inputtedEmail = model.Email;
            string inputtedPassword = model.Password;

            // Check if the Model is valid or not.
            if (!ModelState.IsValid)
            {
                // Model is not valid.
                return RemainInSameViewAndShowValidationError(model, validationResult, Resources.Resources.Error_EmailOrPasswordInvalid, nameof(Login));
            }

            // Model is valid.

            // Check if inputted user exists in database.
            IList<User> users = await _userService.GetByFilterAsync(new UserFilter { Email = inputtedEmail });
            User user = users.SingleOrDefault();

            if (user == null)
            {
                // User not found in the database.
                Log.Info("User not found in the database.");
                validationResult.Errors.Add(string.Empty, Resources.Resources.Error_EmailOrPasswordInvalid);
            }
            else
            {
                // Get role of user.
                user.Role = await GetRoleForUser(user.RoleIdString);
                    
                if (!user.IsConfirmed && user.Role.Name != Constants.Admin)
                {
                    // User Email not confirmed.
                    Log.Info($"User with Email {user.Email} and id {user.Id} is not confirmed.");
                    validationResult.Errors.Add(string.Empty, Resources.Resources.Error_EmailNotYetConfirmed);
                }
                else if (user.IsInactive)
                {
                    // User is set inactive.
                    Log.Info($"User with Email {user.Email} and id {user.Id} is set inactive.");
                    validationResult.Errors.Add(string.Empty, $@"{Resources.Resources.Error_AccountInactive} {Resources.Resources.Error_ContactAdmin}");
                }
                else
                {
                    // User found in the database.
                    Log.Info("User is valid");

                    // Sign in with hashed password.
                    SignInStatus signInStatus = await _userService.SignInWithPasswordAsync(inputtedEmail, inputtedPassword);

                    if (signInStatus == SignInStatus.Success)
                    {
                        _authenticationService.SignIn(inputtedEmail);

                        return RedirectToView(returnUrl);
                    }
                    else if (signInStatus == SignInStatus.Failure)
                    {
                        // Wrong password.
                        Log.Info($"Wrong password entered for user {user} with email {user.Email}.");
                        validationResult.Errors.Add(string.Empty, Resources.Resources.Error_EmailOrPasswordInvalid);
                    }
                }
            }

            // Add ValidationResults to ModelState
            if (validationResult.HasErrors)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);
            }

            // Stay in Login view if validation failed.
            Log.Info("Show Login");
            return View(nameof(Login), model);
        }

        /// <summary>
        /// Logoff Duftfinder
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            _authenticationService.SignOut();

            // Navigate to "Ätherisches Öl suchen" after logoff.
            Log.Info("Redirect to SearchEssentialOil");
            return RedirectToAction("Index", "SearchEssentialOil");
        }

        /// <summary>
        /// Show Register view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers a new user & sends email to user in order for him to verify his account.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Check if the Model is valid or not.
            if (!ModelState.IsValid)
            {
                // Model is not valid.
                return RemainInSameViewAndShowValidationError(model, validationResult, $"{Resources.Resources.Error_RegistrationFailed} {Resources.Resources.Error_NotValidData}", nameof(Register));
            }

            // Model is valid.

            // Check if user with same email already exists.
            IList<User> users = await _userService.GetByFilterAsync(new UserFilter {Email = model.Email});

            if (!users.Any())
            {
                // Map view model to entity.
                User user = new User();
                model.MapViewModelToEntity(user);

                // Create VerifyAccountKey that is sent in verification email.
                user.VerifyAccountKey = Guid.NewGuid();

                // Create new user
                validationResult = await _userService.RegisterAccountAsync(user, model.Password);

                if (user.Id == null)
                {
                    // Only do this for AccountControllerTest. Otherwise user.Id will always be null in test.
                    users = await _userService.GetByFilterAsync(new UserFilter { Email = model.Email });
                    user = users.SingleOrDefault();
                }

                // Prepare account verify email and redirect to RegistrationConfirmation.
                if (user?.Id != null && user?.Email != null && user.VerifyAccountKey != null)
                {
                    validationResult = await PrepareAndSendVerifyAccountEmail(user);
                    Log.Info($"Registered new user with email {user.Email} and id {user.Id}.");
                }
                else
                {
                    validationResult.Errors.Add(string.Empty, $"{Resources.Resources.Error_RegistrationFailed} {Resources.Resources.Error_ContactAdmin}");
                    Log.Info($"Register failed.");
                }

                // Registration & email sending was successful.
                if (!validationResult.HasErrors)
                {
                    Log.Info("Redirect to RegistrationConfirmation");
                    return RedirectToAction(nameof(RegistrationConfirmation));
                }
            }
            else
            {
                // User already exists.
                Log.Info($"Registration failed. User {users.FirstOrDefault()} already exists in database.");
                validationResult.Errors.Add(string.Empty, $@"{Resources.Resources.Error_RegistrationFailed} {Resources.Resources.Error_UserAllreadyRegistered}");
            }

            // Add ValidationResults to ModelState
            if (validationResult.HasErrors)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);
            }

            // Stay in Registration view if validation failed.
            Log.Info("Show Register");
            return View(nameof(Register), model);
        }

        /// <summary>
        /// Show RegistrationConfirmation view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegistrationConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Action is called after user clicks the link in his email, that he received after his registration, in order to verify his registration.
        /// IMPORTANT: Parameters must match with the ones in the verifyAccountUrlFull. Otherwise the action won't be called.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <param name="verifyAccountKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> VerifyAccount(string email, Guid verifyAccountKey)
        {
            ValidationResultList validationResult = await _userService.VerifyAccountAsync(email, verifyAccountKey);

            if (validationResult.HasErrors)
            {
                AddValidationResultsToModelStateErrors(validationResult.Errors);
                Log.Error($"Error occurred while sending info email for admin.");
            }
            else
            {
                Log.Info($"Sent info email to admin about registration of user with email {email}.");
            }

            return View(nameof(VerifyAccount));
        }

        /// <summary>
        /// Show ChangePassword view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword(string email)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel { Email = email };

            return View(nameof(ChangePassword), model);
        }

        /// <summary>
        /// Change password of user.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ValidationResultList validationResult = await _userService.ChangePasswordAsync(model.Email, model.OldPassword, model.Password);

                // Show confirmation page, if password is changed successfully.
                if (!validationResult.HasErrors)
                {
                    return RedirectToAction(nameof(ChangeOrResetPasswordConfirmation), new { email = model.Email });
                }
                
                foreach (KeyValuePair<string, string> error in validationResult.Errors)
                {
                    // Show old password or email validation errors below input field of old password.
                    string fieldName = string.Empty;
                    if (error.Key == Constants.Password)
                    {
                        fieldName = nameof(model.OldPassword);
                    }
                    else if (error.Key == Constants.Email)
                    {
                        fieldName = nameof(model.Email);
                    }

                    ModelState.AddModelError(fieldName, error.Value);
                }
            }

            return View(nameof(ChangePassword), model);
        }

        /// <summary>
        /// Show ChangeOrResetPasswordConfirmation view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeOrResetPasswordConfirmation(string email)
        {
            LoginViewModel model = new LoginViewModel { Email = email };
            return View(model);
        }

        /// <summary>
        /// Show ForgotPassword view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword(string email)
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel { Email = email };

            return View(nameof(ForgotPassword), model);
        }

        /// <summary>
        /// Generates password reset key & sends email to user in order to verify his reset request.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Check if the Model is valid or not.
            if (ModelState.IsValid)
            {
                // Model is valid

                // Check if inputted user exists in database.
                IList<User> users = await _userService.GetByFilterAsync(new UserFilter {Email = model.Email});
                User user = users.SingleOrDefault();

                if (user == null)
                {
                    // User not found in the database.
                    Log.Info("User not found in the database.");
                    validationResult.Errors.Add(string.Empty, Resources.Resources.Error_NoUserWithEmail);
                }
                else
                {
                    // Generate the reset password key for the user.
                    validationResult = await _userService.GenerateAndUpdatePasswordResetKeyAsync(model.Email);

                    // Get user again in order to retrieve password reset key.
                    users = await _userService.GetByFilterAsync(new UserFilter {Email = model.Email});
                    user = users.SingleOrDefault();
                    if (user?.Id != null && user?.Email != null && user.PasswordResetKey != null)
                    {
                        validationResult = await PrepareAndSendResetPasswordEmail(user);
                        Log.Info($"Reset user password with email {user.Email} and id {user.Id}.");
                    }
                    else
                    {
                        validationResult.Errors.Add(string.Empty, $"{Resources.Resources.Error_ResetPasswordFailed} {Resources.Resources.Error_ContactAdmin}");
                        Log.Info($"Forgot password failed.");
                    }

                    // Email sending was successful.
                    if (!validationResult.HasErrors)
                    {
                        Log.Info("Redirect to ForgotPasswordConfirmation");
                        return RedirectToAction(nameof(ForgotPasswordConfirmation));
                    }
                }

                // Add ValidationResults to ModelState.
                if (validationResult.HasErrors)
                {
                    AddValidationResultsToModelStateErrors(validationResult.Errors);
                }
            }

            // Stay in same view if validation failed.
            Log.Info($"Show {nameof(ForgotPassword)}");
            return View(nameof(ForgotPassword), model);
        }

        /// <summary>
        /// Show ForgotPasswordConfirmation view.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Action is called after user clicks the link in his email, that he received after his forgot password request.
        /// IMPORTANT: Parameters must match with the ones in the passwordResetUrlFull. Otherwise the action won't be called.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <param name="passwordResetKey"></param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ResetPassword(string email, Guid passwordResetKey)
        {
            // Fill values with data from url.
            ResetPasswordViewModel model = new ResetPasswordViewModel { Email = email, PasswordResetKey = passwordResetKey };

            return View(nameof(ResetPassword), model);
        }

        /// <summary>
        /// Reset password after user has inputted his new password. Is called after save click.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ValidationResultList validationResult = await _userService.ResetPasswordAsync(model.Email, model.Password, model.PasswordResetKey);

                if (validationResult.HasErrors)
                {
                    AddValidationResultsToModelStateErrors(validationResult.Errors);

                    Log.Info($"ResetPassword failed for email {model.Email}.");
                }
                else
                {
                    Log.Info($"ResetPassword successful for email {model.Email}.");
                    Log.Info("Redirect to ChangeOrResetPasswordConfirmation");
                    return RedirectToAction(nameof(ChangeOrResetPasswordConfirmation), new { email = model.Email });
                }
            }

            // Stay in same view if validation failed.
            Log.Info($"Show {nameof(ResetPassword)}");
            return View(nameof(ResetPassword), model);

        }

        /// <summary>
        /// Prepares and sends the email, that is sent to the user in order to reset his password.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> PrepareAndSendResetPasswordEmail(User user)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Get url from user mail & password reset key.
            string passwordResetPath = Url.Action("ResetPassword", "Account", new { email = user.Email, passwordResetKey = user.PasswordResetKey });
            string passwordResetUrlFull = Url.Action("ResetPassword", "Account", new { email = user.Email, passwordResetKey = user.PasswordResetKey }, Request?.Url?.Scheme) ?? passwordResetPath;

            Log.Info($"passwordResetUrlFull is {passwordResetUrlFull}");

            if (passwordResetUrlFull != null)
            {
                // Prepare the email & send the email to the user.
                validationResult = await _userService.GenerateAndSendMailForUserAsync(user, new Uri(passwordResetUrlFull), Constants.ForgotPasswordEmailSubject, Constants.ForgotPasswordEmailText);
            }

            return validationResult;
        }

        /// <summary>
        /// Prepares and sends the email, that is sent to the new registered user in order to verify his account.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> PrepareAndSendVerifyAccountEmail(User user)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Get url from user mail & confirmation key.
            string verifyAccountUrl = Url.Action("VerifyAccount", "Account", new { email = user.Email, verifyAccountKey = user.VerifyAccountKey });
            string verifyAccountUrlFull = Url.Action("VerifyAccount", "Account", new { email = user.Email, verifyAccountKey = user.VerifyAccountKey }, Request?.Url?.Scheme) ?? verifyAccountUrl;

            Log.Info($"verifyAccountUrlFull is {verifyAccountUrlFull}");

            if (verifyAccountUrlFull != null)
            {
                // Prepare the email & send the email to the user.
                validationResult = await _userService.GenerateAndSendMailForUserAsync(user, new Uri(verifyAccountUrlFull), Constants.VerifyAccountEmailSubject, Constants.VerifyAccountEmailText);
            }

            return validationResult;
        }

        /// <summary>
        /// Returns role of user, is one is set.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="userRoleIdString"></param>
        /// <returns></returns>
        private async Task<Role> GetRoleForUser(string userRoleIdString)
        {
            if (userRoleIdString != null)
            {
                return await _userService.GetRoleForUserAsync(userRoleIdString);
            }
            return new Role();
        }

        /// <summary>
        /// Redirect to appropriate view according to which url was entered when navigated to login page.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToView(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                // Navigate to specific url.
                Log.Info($"Redirect to local url {returnUrl}");
                return Redirect(returnUrl);
            }

            // Navigate to "Ätherisches Öl suchen" if no specific url was entered.
            Log.Info("Redirect to SearchEssentialOil");
            return RedirectToAction("Index", "SearchEssentialOil");
        }

        /// <summary>
        /// Remains in same view if form not valid and adds validation error.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="model"></param>
        /// <param name="validationResult"></param>
        /// <param name="errorMessage"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private ActionResult RemainInSameViewAndShowValidationError(LoginViewModel model, ValidationResultList validationResult, string errorMessage, string viewName)
        {
            // Stay in view if ModelState is invalid, redisplay form.
            Log.Info("ModelState is invalid.");
            validationResult.Errors.Add(string.Empty, errorMessage);
            AddValidationResultsToModelStateErrors(validationResult.Errors);

            // Stay in same view if validation failed.
            Log.Info($"Show {viewName}");
            return View(viewName, model);
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