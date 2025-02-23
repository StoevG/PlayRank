using System.Net;
using System.Text.Json;
using PlayRank.Api.ExceptionHandling.Interfaces;

namespace PlayRank.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandler _exceptionHandlerChain;

        public ExceptionHandlingMiddleware(RequestDelegate next, IExceptionHandler exceptionHandlerChain)
        {
            _next = next;
            _exceptionHandlerChain = exceptionHandlerChain;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var handled = await _exceptionHandlerChain.HandleAsync(ex, context);
                if (!handled)
                {
                    await HandleFallbackAsync(ex, context);
                }
            }
        }

        private static Task HandleFallbackAsync(Exception ex, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            return context.Response.WriteAsync(result);
        }
    }
}
