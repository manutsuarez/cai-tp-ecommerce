using Microsoft.AspNetCore.Diagnostics;
using Orders.API.DTOs;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

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

        ErrorResponse respuesta;

        if (ex.ErrorCode == ErrorCodes.ORD_005)
        {
            respuesta = new ErrorResponse
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
        }
        else
        {
            respuesta = new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = "No se puede modificar el estado.",
                Instance = httpContext.Request.Path.Value,
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message,
                CorrelationId = httpContext.Items["X-Correlation-Id"] as string
            };
        }

        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}