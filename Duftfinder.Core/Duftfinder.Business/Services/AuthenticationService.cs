using System.Reflection;
using Duftfinder.Domain.Interfaces.Services;
using log4net;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains business logic for authentication related stuff. 
    /// </summary>
    /// <seealso>adesso SzkB.Ehypo project</seealso> 
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void SignIn(string email)
        {
			// TODO
            //FormsAuthentication.SetAuthCookie(email, false);
            Log.Info("SignIn");
        }

        public void SignOut()
        {
			// TODO
            //FormsAuthentication.SignOut();
            Log.Info("SignOut");
        }
    }
}
