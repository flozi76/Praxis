using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Duftfinder.Web.Helpers
{
	/// <summary>
	///     HtmlHelper class for all kind of stuff.
	/// </summary>
	/// <author>Anna Krebs</author>
	public static class DuftfinderHtmlHelper
	{
		/// <summary>
		///     Gets assembly version in format x.x.x .
		/// </summary>
		/// <author>Anna Krebs</author>
		/// <param name="helper"></param>
		/// <returns></returns>
		public static HtmlString ApplicationVersion(this HtmlHelper helper)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;

			if (version != null) return new HtmlString($"{version.Major}.{version.Minor}.{version.Build}");
			return new HtmlString("");
		}
	}
}