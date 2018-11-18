using Duftfinder.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for Registration that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class RegisterViewModel : LoginViewModel
    {
        public RegisterViewModel()
        {
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InputRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InputRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [StringLength(16, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InvalidPasswordConfirmation")]
        [DataType(DataType.Password)]
        public string Password2 { get; set; }

        /// <summary>
        /// Map values from View to Entity.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="user"></param>
        public void MapViewModelToEntity(User user)
        {
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
        }
    }

}

