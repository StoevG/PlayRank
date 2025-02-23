using System.Net;
using System.Text.Json;

namespace PlayRank.Api.ExceptionHandling
{
    public class ValidationExceptionHandler : ExceptionHandlerBase
    {
        protected override bool CanHandle(Exception exception)
        {
            return exception is ArgumentException;
        }

        protected override Task HandleExceptionAsync(Exception exception, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var result = JsonSerializer.Serialize(new { error = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
