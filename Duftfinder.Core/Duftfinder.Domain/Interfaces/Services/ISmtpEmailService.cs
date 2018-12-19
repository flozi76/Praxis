using System.Threading.Tasks;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service for sending email related stuff.
	/// </summary>
	/// <seealso>adesso SzkB.Ehypo project</seealso>
	public interface ISmtpEmailService
	{
		Task<ValidationResultList> SendEmailAsync(Email email, bool isValidationErrorVisibleForAdmin);
	}
}