using System.Net;
using System.Text.Json;

namespace PlayRank.Api.ExceptionHandling
{
    public class NotFoundExceptionHandler : ExceptionHandlerBase
    {
        protected override bool CanHandle(Exception exception)
        {
            return exception is KeyNotFoundException;
        }

        protected override Task HandleExceptionAsync(Exception exception, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            var result = JsonSerializer.Serialize(new { error = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
