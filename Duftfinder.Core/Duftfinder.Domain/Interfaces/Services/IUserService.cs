using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
    /// <summary>
    /// Represents the interface for the business logic service for user related stuff.
    /// </summary>
    /// <author>Anna Krebs</author>
    public interface IUserService : IService<User, UserFilter>
    {
        Task<Role> GetRoleForUserAsync(string roleId);

        Task<IList<Role>> GetAllRolesAsync(RoleFilter filter);

        Task<ValidationResultList> RegisterAccountAsync(User user, string password);

        Task<SignInStatus> SignInWithPasswordAsync(string email, string password);

        Task<ValidationResultList> ChangePasswordAsync(string email, string oldPassword, string newPassword);

        Task<ValidationResultList> GenerateAndUpdatePasswordResetKeyAsync(string email);
        
        Task<ValidationResultList> ResetPasswordAsync(string email, string password, Guid passwordResetKey);

        Task<ValidationResultList> VerifyAccountAsync(string email, Guid verifyAccountKey);

        Task<ValidationResultList> GenerateAndSendMailForUserAsync(User user, Uri emailUri, string emailSubjectConfigurationKey, string emailTextConfigurationKey);
    }
}
