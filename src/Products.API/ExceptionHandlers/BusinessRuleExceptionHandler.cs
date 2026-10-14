
using Microsoft.AspNetCore.Diagnostics;
using Products.API.DTOs;
using Products.API.Exceptions;

namespace Products.API.ExceptionHandlers;

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
            Detail = "No se pudo completar la operación.",
            Instance = httpContext.Request.Path.Value,
            ErrorCode = ex.ErrorCode,
            ErrorMessage = ex.Message
        };

        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true;
    }
}