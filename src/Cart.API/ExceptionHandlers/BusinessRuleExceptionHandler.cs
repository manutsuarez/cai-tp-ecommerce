using Microsoft.AspNetCore.Diagnostics;
using Cart.API.DTOs;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers;

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
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "Unprocessable Entity",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "No se puede procesar la solicitud.",
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