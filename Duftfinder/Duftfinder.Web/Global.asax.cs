using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Interfaces.Services;
using Duftfinder.Ioc.Helpers;
using Duftfinder.Web.App_Start;
using Duftfinder.Web.Controllers;
using log4net;
using WebGrease.Activities;
using Unity;

namespace Duftfinder.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register Unity container
            ContainerConfig.Initialize();

            // Add HandleErrorAttribute in order to show custom error page.
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }
        
        protected void Application_PostAuthenticateRequest()
        {
            GetAuthorizationForUser();
        }

        /// <summary>
        /// Determines what functionality user with specific role has access to.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <seealso> href="https://www.codeproject.com/Articles/578374/AplusBeginner-splusTutorialplusonplusCustomplusF">codeproject</seealso> 
        private void GetAuthorizationForUser()
        {
            IUserService userService = DependencyResolver.Current.GetService<IUserService>();

            UserFilter filter = new UserFilter();

            // Get values from database.
            IList<User> users = Task.Run(() => userService.GetAllAsync(filter)).Result;

            if (!FormsAuthentication.CookiesSupported || Request.Cookies[FormsAuthentication.FormsCookieName] == null)
            {
                return;
            }

            // Check authentication and authorization with respect to the current role.
            try
            {
                // Get the email of the logged in user.
                string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value)?.Name;

                User user = users.SingleOrDefault(u => u.Email == email);
                string roles = null;
                if (user != null)
                {
                    // Get the role of the user.
                    user.Role = Task.Run(() => userService.GetRoleForUserAsync(user.RoleIdString)).Result;
                    if (user.Role != null)
                    {
                        roles = user.Role.Name;
                    }
                }

                // Set the Principal with the user specific details.
                if (roles != null && email != null)
                {
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(email, "Forms"), roles.Split(';'));
                }
            }
            catch (Exception e)
            {
                Log.Error($"An unexpected error occurred while checking authentication and authorization. Exception: {e.Message}");
            }
        }
    }
}