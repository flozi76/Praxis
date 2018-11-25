using System;
using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Passwort zurücksetzen" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class ResetPasswordViewModel : LoginViewModel
    {
        public ResetPasswordViewModel()
        {
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [StringLength(16, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPasswordConfirmation")]
        [DataType(DataType.Password)]
        public string Password2 { get; set; }

        public Guid PasswordResetKey { get; set; }

    }

}

