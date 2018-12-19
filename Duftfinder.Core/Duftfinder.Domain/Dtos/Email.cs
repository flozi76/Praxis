using System.ComponentModel.DataAnnotations;
using Duftfinder.Domain.Entities;

namespace Duftfinder.Domain.Dtos
{
	/// <summary>
	///     Represents the Email.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class Email
	{
		public User User { get; set; }

		[Required] public string EmailAddress { get; set; }

		public string Subject { get; set; }

		public string EmailHtmlText { get; set; }
	}
}