using System.Net;
using System.Text.Json;

namespace PlayRank.Api.ExceptionHandling
{
    public class GenericExceptionHandler : ExceptionHandlerBase
    {
        protected override bool CanHandle(Exception exception)
        {
            return true;
        }

        protected override Task HandleExceptionAsync(Exception exception, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred.", details = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
