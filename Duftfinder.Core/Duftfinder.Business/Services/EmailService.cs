using System;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for email related stuff. 
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso>adesso SzkB.Ehypo project</seealso>
    public class EmailService : IEmailService
    {
        private readonly IConfigurationService _configurationService;

        public EmailService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Generates email with the subject & the text from the configuration collection for user.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso>adesso SzkB.Ehypo project</seealso>
        /// <param name="user"></param>
        /// <param name="emailUri"></param>
        /// <param name="emailSubjectConfigurationKey"></param>
        /// <param name="emailTextConfigurationKey"></param>
        /// <returns></returns>
        public async Task<Email> GenerateEmailForUser(User user, Uri emailUri, string emailSubjectConfigurationKey, string emailTextConfigurationKey)
        {
            // Get the subject & the text for the email.
            string emailSubject = await _configurationService.GetConfigurationParameterByKeyAsync(emailSubjectConfigurationKey);
            string emailText = await _configurationService.GetConfigurationParameterByKeyAsync(emailTextConfigurationKey);

            // Put together the email message & and the uri, that will be shown in the email.
            string emailBody = string.Format(emailText, emailUri?.AbsoluteUri);

            // Create the email that wil be sent.
            Email email = new Email();
            email.User = user;
            email.EmailAddress = user.Email;
            email.Subject = emailSubject;
            email.EmailHtmlText = emailBody;

            return email;
        }

        /// <summary>
        /// Generates email with the subject & the text from the configuration collection for admin. 
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="userEmail"></param>
        /// <param name="emailSubjectConfigurationKey"></param>
        /// <param name="emailTextConfigurationKey"></param>
        /// <returns></returns>
        public async Task<Email> GenerateInfoEmailForAdmin(string userEmail, string emailSubjectConfigurationKey, string emailTextConfigurationKey)
        {
            // Get the subject & the text for the email.
            string emailSubject = await _configurationService.GetConfigurationParameterByKeyAsync(emailSubjectConfigurationKey);
            string emailText = await _configurationService.GetConfigurationParameterByKeyAsync(emailTextConfigurationKey);
            string adminEmail = await _configurationService.GetConfigurationParameterByKeyAsync(Constants.SmtpEmailUser);

            // Put together the email message & user email.
            string emailBody = string.Format(emailText, userEmail);

            // Create the email that wil be sent.
            Email email = new Email();
            email.EmailAddress = adminEmail;
            email.Subject = emailSubject;
            email.EmailHtmlText = emailBody;

            return email;
        }
    }
}
