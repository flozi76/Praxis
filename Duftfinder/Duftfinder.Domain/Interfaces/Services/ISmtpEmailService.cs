using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service for sending email related stuff.
    /// </summary>
    /// <seealso>adesso SzkB.Ehypo project</seealso>
    public interface ISmtpEmailService
    {
        Task<ValidationResultList> SendEmailAsync(Email email, bool isValidationErrorVisibleForAdmin);
    }
}
