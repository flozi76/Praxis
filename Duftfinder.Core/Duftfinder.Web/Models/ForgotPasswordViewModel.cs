using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Passwort vergessen" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class ForgotPasswordViewModel
    {
        public ForgotPasswordViewModel()
        {
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmail")]
        [StringLength(40, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmailLength")]
        public string Email { get; set; }
    }
}

