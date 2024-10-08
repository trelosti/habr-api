using Microsoft.AspNetCore.Mvc;

namespace Habr.Common.Exceptions
{
    public interface IExceptionMapper
    {
        ProblemDetails Map(Exception exception);
    }
}
