namespace PlayRank.Api.ExceptionHandling.Interfaces
{
    public interface IExceptionHandler
    {

        /// <summary>
        /// Sets the next handler in the chain.
        /// </summary>
        Task<bool> HandleAsync(Exception exception, HttpContext context);


        /// <summary>
        /// Sets the next handler in the chain.
        /// </summary>
        IExceptionHandler SetNext(IExceptionHandler next);
    }
}
