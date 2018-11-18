using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service for email related stuff.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso>adesso SzkB.Ehypo project</seealso>
    public interface IEmailService
    {
        Task<Email> GenerateEmailForUser(User user, Uri emailUri, string emailSubjectConfigurationKey, string emailTextConfigurationKey);

        Task<Email> GenerateInfoEmailForAdmin(string userEmail, string emailSubjectConfigurationKey, string emailTextConfigurationKey);
    }
}
