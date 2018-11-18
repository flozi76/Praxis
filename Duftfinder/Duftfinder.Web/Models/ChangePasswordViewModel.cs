using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Passwort ändern" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class ChangePasswordViewModel : LoginViewModel
    {
        public ChangePasswordViewModel()
        {
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_OldPasswordRequired")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [StringLength(16, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPasswordConfirmation")]
        [DataType(DataType.Password)]
        public string Password2 { get; set; }
    }

}

