using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Investran.Api.Attributes
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var message = $"[{actionExecutedContext.Request.Method.Method}] " +
                          $"[{actionExecutedContext.Request.RequestUri.AbsoluteUri}] " +
                          $"{actionExecutedContext.Exception.Message}";
            Logs.Error(message);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"{actionExecutedContext.Exception.Message}"),
                ReasonPhrase = "Server Error Occurred. Please Contact your Administrator."
            };
            actionExecutedContext.Response = responseMessage;
        }
    }
}
