using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BusinessRuleException ex)
        {
            return false;
        }

        ErrorResponse respuesta = new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            Title = "Conflict",
            Status = StatusCodes.Status409Conflict,
            Detail = "Ya existe un recurso con esos datos.",
            Instance = httpContext.Request.Path.Value,
            ErrorCode = ex.ErrorCode,
            ErrorMessage = ex.Message,
            CorrelationId = httpContext.Items["X-Correlation-Id"] as string
        };

        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}