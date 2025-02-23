using PlayRank.Api.ExceptionHandling;
using PlayRank.Api.Middlewares;

namespace PlayRank.Api.Extensions
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder builder)
        {
            var notFoundHandler = new NotFoundExceptionHandler();
            var validationHandler = new ValidationExceptionHandler();
            var genericHandler = new GenericExceptionHandler();

            notFoundHandler
                .SetNext(validationHandler)
                .SetNext(genericHandler);

            return builder.UseMiddleware<ExceptionHandlingMiddleware>(notFoundHandler);
        }
    }
}
