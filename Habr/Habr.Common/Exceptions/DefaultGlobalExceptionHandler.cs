using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Habr.Common.Exceptions
{
    public sealed class DefaultGlobalExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionMapper _exceptionMapper;

        public DefaultGlobalExceptionHandler(IExceptionMapper exceptionMapper)
        {
            _exceptionMapper = exceptionMapper;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = _exceptionMapper.Map(exception);

            await Results.Problem(problemDetails).ExecuteAsync(httpContext);

            return true;
        }
    }
}
