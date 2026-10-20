using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Users.API.DTOs;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers;

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

        Log.Warning("Regla de negocio violada. ErrorCode {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);

        ErrorResponse respuesta;

        if (ex.ErrorCode == ErrorCodes.USR_003)
        {
            respuesta = new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Las credenciales no son válidas.",
                Instance = httpContext.Request.Path.Value,
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message,
                CorrelationId = httpContext.Items["X-Correlation-Id"] as string
            };
        }
        else if (ex.ErrorCode == ErrorCodes.USR_004 || ex.ErrorCode == ErrorCodes.USR_005)
        {
            respuesta = new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "El acceso está prohibido.",
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
                Detail = "Ya existe un recurso con esos datos.",
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