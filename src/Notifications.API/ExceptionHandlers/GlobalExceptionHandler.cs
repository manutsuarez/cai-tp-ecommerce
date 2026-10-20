using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers;

public class GlobalExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        Log.Error(exception, "Error inesperado. ErrorCode {ErrorCode}", ErrorCodes.NTF_004);

        string detalle = "Ocurrió un error inesperado.";
        if (environment.IsDevelopment())
        {
            detalle = exception.Message;
        }

        ErrorResponse respuesta = new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = detalle,
            Instance = httpContext.Request.Path.Value,
            ErrorCode = ErrorCodes.NTF_004,
            ErrorMessage = "Error interno al procesar la notificación.",
            CorrelationId = httpContext.Items["X-Correlation-Id"] as string
        };

        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}