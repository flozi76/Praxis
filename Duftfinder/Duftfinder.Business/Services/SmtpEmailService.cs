using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Repositories;
using Duftfinder.Domain.Interfaces.Services;
using log4net;
using System.Web.Security;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for sending email related stuff. 
    /// </summary>
    /// <seealso>adesso SzkB.Ehypo project</seealso>
	public class SmtpEmailService : ISmtpEmailService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IConfigurationService _configurationService;

        public SmtpEmailService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Sends email with the configured parameters.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isValidationErrorVisibleForAdmin"></param>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <returns></returns>
        public async Task<ValidationResultList> SendEmailAsync(Email email, bool isValidationErrorVisibleForAdmin)
        {
            ValidationResultList validationResult = new ValidationResultList();

            // Read Email settings from Configuration collection in database.
            string smtpServer = await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpServer);
            int smtpPort = Convert.ToInt32(await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpPort));
            bool smtpEnableSsl = Convert.ToBoolean(await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpEnableSsl));
            string smtpEmailUser = await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpEmailUser);
            string smtpEmailUserPassword = await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpEmailUserPassword);
            string smtpEmailFrom = await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpEmailFrom);

            // Setup the smtp client with the given configuration.
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort) { Credentials = new NetworkCredential(smtpEmailUser, smtpEmailUserPassword), EnableSsl = smtpEnableSsl })
            {
                try
                {
                    using (MailMessage message = new MailMessage(smtpEmailFrom, email.EmailAddress))
                    {
                        message.Body = email.EmailHtmlText;
                        message.IsBodyHtml = true;
                        message.SubjectEncoding = Encoding.UTF8;
                        message.Subject = email.Subject;

                        // Send the email.
                        await client.SendMailAsync(message);

                        Log.Info($"SendEmailAsync was successful. E-Mail with subject {email.Subject} was sent to user {email.User}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"SendEmailAsync failed. Exception Message: {ex.Message}");
                    // Show text "contact admin" according to whether the validation message is shown for the user or the admin.
                    string contactAdminText = !isValidationErrorVisibleForAdmin ? Resources.Resources.Error_ContactAdmin : string.Empty;
                    validationResult.Errors.Add(Constants.SendEmailValidationError, $"{Resources.Resources.Error_SendingEmailFailed} {contactAdminText}");
                }

                return validationResult;
            }
        }
    }
}
