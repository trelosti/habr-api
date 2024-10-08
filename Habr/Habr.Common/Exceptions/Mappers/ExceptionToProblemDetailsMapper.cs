using Habr.Common.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace Habr.Common.Exceptions.Mappers
{
    public sealed class ExceptionToProblemDetailsMapper : IExceptionMapper
    {
        public ProblemDetails Map(Exception exception)
        {
            var problemDetails = exception switch
            {
                AuthenticationException authenticationException => new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = CommonMessageResource.Unauthorized,
                    Detail = exception.Message
                },
                DbUpdateConcurrencyException dbUpdateConcurrencyException => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = CommonMessageResource.ConcurrencyConflict,
                    Detail = exception.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = CommonMessageResource.InternalServerError,
                    Detail = exception.Message
                }
            };

            return problemDetails;
        }
    }
}
