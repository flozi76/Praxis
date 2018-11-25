using System.Net;
using System.Web.Mvc;

namespace Duftfinder.Web.Helpers
{
    /// <summary>
    /// Helper to displays json errors in javascript.
    /// </summary>
    /// <seealso> href="https://stackoverflow.com/questions/11370251/return-json-with-error-status-code-mvc/11370571">stackoverflow</seealso> 
    public class JsonErrorResult : JsonResult
    {
        public JsonErrorResult(object data)
        {
            Data = data;
        }

        public JsonErrorResult(string errorMessage)
        {
            Data = new { errorMessage };
        }

        public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

        public override void ExecuteResult(ControllerContext context)
        {
            context.RequestContext.HttpContext.Response.StatusCode = (int)StatusCode;
            base.ExecuteResult(context);
        }
    }
}