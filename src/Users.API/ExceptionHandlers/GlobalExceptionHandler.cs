using Microsoft.AspNetCore.Diagnostics;
using Users.API.DTOs;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers;

public class GlobalExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
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
            ErrorCode = ErrorCodes.USR_006,
            ErrorMessage = "Error interno al procesar el usuario.",
            CorrelationId = httpContext.Items["X-Correlation-Id"] as string
        };

        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}