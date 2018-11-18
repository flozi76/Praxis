using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: Constants.Default,
                url: Constants.RouteUrl,
                defaults: new { controller = Constants.SearchEssentialOil, action = Constants.Index, id = UrlParameter.Optional }
            );
        }
    }
}
