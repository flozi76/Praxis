using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using log4net;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for user related stuff. 
    /// </summary>
    /// <author>Anna Krebs</author>
    public class UserService : Service<User, UserFilter, IUserRepository>, IUserService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICryptoService _cryptoService;

        private readonly IUserRepository _userRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IEmailService _emailService;

        private readonly ISmtpEmailService _smtpEmailService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ICryptoService cryptoService, IEmailService emailService, ISmtpEmailService smtpEmailService)
            : base(userRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cryptoService = cryptoService;
            _emailService = emailService;
            _smtpEmailService = smtpEmailService;
        }

        /// <summary>
        /// Gets the role for specific user.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Role> GetRoleForUserAsync(string roleId)
        {
            Role role = await _roleRepository.GetByIdAsync(roleId);

            return role;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IList<Role>> GetAllRolesAsync(RoleFilter filter)
        {
            IList<Role> roles = await _roleRepository.GetAllAsync(filter);

            return roles;
        }

        /// <summary>
        /// Register new account in database.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> RegisterAccountAsync(User user, string password)
        {
            ValidationResultList validationResult = new ValidationResultList();
            IList<User> userExisting = await _userRepository.GetByFilterAsync(new UserFilter {Email = user.Email});

            // Check if user with same email already exists.
            if (userExisting.Any())
            {
                // User already exist.
                validationResult.Errors.Add(string.Empty, Resources.Resources.Error_UserAllreadyRegistered);
            }
            else
            {
                // Create new user with hashed password.
                user.PasswordHash = _cryptoService.GeneratePasswordHash(password);
                validationResult = await Repository.InsertAsync(user);
            }

            Log.Info($"RegisterAccountAsync for email {user.Email}. Validation Result has errors: {validationResult.HasErrors}");
            return validationResult;
        }

        /// <summary>
        /// Signs in user with hashed password.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<SignInStatus> SignInWithPasswordAsync(string email, string password)
        {
            IList<User> users = await Repository.GetByFilterAsync(new UserFilter {Email = email});
            User user = users.SingleOrDefault();

            SignInStatus result = SignInStatus.Failure;

            // Check if entered password corresponds with password hash.
            if (user != null)
            {
                if (_cryptoService.ValidatePassword(password, user.PasswordHash))
                {
                    result = SignInStatus.Success;
                }
                else
                {
                    result = SignInStatus.Failure;
                }
            }

            return result;
        }

        /// <summary>
        /// Changes password.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="email"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            ValidationResultList validationResult = new ValidationResultList();

            IList<User> users = await Repository.GetByFilterAsync(new UserFilter { Email = email });
            User user = users.SingleOrDefault();

            if (user == null)
            {
                validationResult.Errors.Add(Constants.Email, Resources.Resources.Error_NoUserWithEmail);
                return validationResult;
            }
           
            if (!_cryptoService.ValidatePassword(oldPassword, user.PasswordHash))
            {
                validationResult.Errors.Add(Constants.Password, Resources.Resources.Error_OldPasswordInvalid);
                return validationResult;
            }

            // Generate new password & update user.
            string hashedPassword = _cryptoService.GeneratePasswordHash(newPassword);
            user.PasswordHash = hashedPassword;
            validationResult =  await Repository.UpdateAsync(user);

            Log.Info($"ChangePasswordAsync for email {user.Email}. Validation Result has errors: {validationResult?.HasErrors}");
            return validationResult;
        }

        /// <summary>
        /// Generates a PasswordResetKey, which is sent to user in order to change his password, when forgotten.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> GenerateAndUpdatePasswordResetKeyAsync(string email)
        {
            ValidationResultList validationResult = new ValidationResultList();

            IList<User> users = await Repository.GetByFilterAsync(new UserFilter { Email = email });
            User user = users.SingleOrDefault();

            if (user == null)
            {
                validationResult.Errors.Add(Constants.Email, Resources.Resources.Error_NoUserWithEmail);
                return validationResult;
            }

            user.PasswordResetKey = Guid.NewGuid();

            return await Repository.UpdateAsync(user);
        }

        /// <summary>
        /// Resets the password of the user, after he requested a reset after ForgotPassword.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="passwordResetKey"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> ResetPasswordAsync(string email, string password, Guid passwordResetKey)
        {
            ValidationResultList validationResult = new ValidationResultList();

            IList<User> users = await Repository.GetByFilterAsync(new UserFilter { Email = email });
            User user = users.SingleOrDefault();

            if (user == null)
            {
                validationResult.Errors.Add(Constants.Email, Resources.Resources.Error_NoUserWithEmail);
                return validationResult;
            }

            // Exception, if reset key was not found in database.
            if (user.PasswordResetKey != passwordResetKey)
            {
                Log.Error($"Password for email {email} cannot be reset using key {passwordResetKey}");
                throw new InvalidOperationException($"Password for email {email} cannot be reset using key {passwordResetKey}");
            }

            string hashedPassword = _cryptoService.GeneratePasswordHash(password);
            user.PasswordHash = hashedPassword;
            user.PasswordResetKey = null;

            validationResult = await Repository.UpdateAsync(user);

            Log.Info($"ResetPasswordAsync for email {email}. Validation Result has errors: {validationResult?.HasErrors}");
            return validationResult;
        }

        /// <summary>
        /// Sets the user account to  state "verified", after user has verified his account using the link in the email.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="email"></param>
        /// <param name="verifyAccountKey"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> VerifyAccountAsync(string email, Guid verifyAccountKey)
        {
            IList<User> users = await Repository.GetByFilterAsync(new UserFilter { Email = email });
            User user = users.SingleOrDefault();

            bool isFirstClickOnLink = user?.VerifyAccountKey == verifyAccountKey;

            // Exception, if reset key was not found in database.
            if ((user == null) || (user.VerifyAccountKey != verifyAccountKey && !user.IsAccountVerified))
            {
                Log.Error($"Email {email} cannot be verified using verification key {verifyAccountKey}");
                throw new InvalidOperationException($"Email {email} cannot be verified using verification key {verifyAccountKey}");
            }

            user.IsAccountVerified = true;
            user.VerifyAccountKey = null;
            ValidationResultList validationResult = await Repository.UpdateAsync(user);

            if (!validationResult.HasErrors && isFirstClickOnLink)
            {
                // Send E-Mail to admin, the first time the user clicked the link.
                validationResult = await GenerateAndSendMailForAdminAsync(email, Constants.InfoAboutRegistrationEmailSubject, Constants.InfoAboutRegistrationEmailText);
            }

            return validationResult;
        }

        /// <summary>
        /// Sends info mail to admin.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <param name="emailSubjectConfigurationKey"></param>
        /// <param name="emailTextConfigurationKey"></param>
        /// <returns></returns>
        private async Task<ValidationResultList> GenerateAndSendMailForAdminAsync(string email, string emailSubjectConfigurationKey, string emailTextConfigurationKey)
        {
            // Prepare the email, with sender, subject, email text etc.
            Email infoEmailForAdmin = await _emailService.GenerateInfoEmailForAdmin(email, emailSubjectConfigurationKey, emailTextConfigurationKey);

            // Actually send the email.
            ValidationResultList validationResult = await _smtpEmailService.SendEmailAsync(infoEmailForAdmin, false);

            return validationResult;
        }

        /// <summary>
        /// Sends info mail to user.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="user"></param>
        /// <param name="emailUri"></param>
        /// <param name="emailSubjectConfigurationKey"></param>
        /// <param name="emailTextConfigurationKey"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> GenerateAndSendMailForUserAsync(User user, Uri emailUri, string emailSubjectConfigurationKey, string emailTextConfigurationKey)
        {
            // Prepare the email, with sender, subject, email text etc.
            Email email = await _emailService.GenerateEmailForUser(user, emailUri, emailSubjectConfigurationKey, emailTextConfigurationKey);

            // Actually send the email.
            ValidationResultList validationResult = await _smtpEmailService.SendEmailAsync(email, false);

            return validationResult;
        }
    }
}