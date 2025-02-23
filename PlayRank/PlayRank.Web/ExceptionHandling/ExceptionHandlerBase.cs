using PlayRank.Api.ExceptionHandling.Interfaces;

namespace PlayRank.Api.ExceptionHandling
{
    public abstract class ExceptionHandlerBase : IExceptionHandler
    {
        private IExceptionHandler _nextHandler;

        public IExceptionHandler SetNext(IExceptionHandler next)
        {
            _nextHandler = next;
            return next;
        }

        public async Task<bool> HandleAsync(Exception exception, HttpContext context)
        {
            if (CanHandle(exception))
            {
                await HandleExceptionAsync(exception, context);
                return true;
            }
            else if (_nextHandler != null)
            {
                return await _nextHandler.HandleAsync(exception, context);
            }
            return false;
        }

        protected abstract bool CanHandle(Exception exception);

        protected abstract Task HandleExceptionAsync(Exception exception, HttpContext context);
    }

}
