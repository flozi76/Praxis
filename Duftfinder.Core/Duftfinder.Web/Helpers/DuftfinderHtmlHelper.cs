using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Duftfinder.Web.Helpers
{
    /// <summary>
    /// HtmlHelper class for all kind of stuff.
    /// </summary>
    /// <author>Anna Krebs</author>
    public static class DuftfinderHtmlHelper
    {
        /// <summary>
        /// Gets assembly version in format x.x.x .
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static HtmlString ApplicationVersion(this HtmlHelper helper)
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;

            if (version != null)
            {
                return new HtmlString($"{version.Major}.{version.Minor}.{version.Build}");
            }
            return new HtmlString("");
        }
    }
}