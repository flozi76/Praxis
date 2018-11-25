using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for Login that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class LoginViewModel
    {
        public LoginViewModel()
        {
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmail")]
        [StringLength(40, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidEmailLength")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [StringLength(16, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}

