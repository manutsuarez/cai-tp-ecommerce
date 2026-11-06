using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Cart.API.DTOs;
using Cart.API.Exceptions;

namespace Cart.API.Configuracion;

/// <summary>
/// Configura qué responde la API cuando el body del pedido no pasa las validaciones
/// (los atributos de Data Annotations, como [Required]): un 400 con el formato de error
/// del enunciado y el código CRT-004.
/// </summary>
public class ConfiguracionApi : IConfigureOptions<ApiBehaviorOptions>
{
    public void Configure(ApiBehaviorOptions options)
    {
        // Le pasamos a .NET el NOMBRE del método que arma la respuesta
        options.InvalidModelStateResponseFactory = CrearRespuestaDeDatosInvalidos;
    }

    private static IActionResult CrearRespuestaDeDatosInvalidos(ActionContext context)
    {
        List<string> mensajes = new List<string>();
        foreach (var entrada in context.ModelState.Values)
        {
            foreach (var error in entrada.Errors)
            {
                mensajes.Add(error.ErrorMessage);
            }
        }

        Log.Warning("Datos inválidos. ErrorCode {ErrorCode}: {ErrorMessage}", ErrorCodes.CRT_004, string.Join("; ", mensajes));

        ErrorResponse respuesta = new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Los datos enviados son inválidos.",
            Instance = context.HttpContext.Request.Path.Value,
            ErrorCode = ErrorCodes.CRT_004,
            ErrorMessage = string.Join("; ", mensajes),
            CorrelationId = context.HttpContext.Items["X-Correlation-Id"] as string
        };

        return new ObjectResult(respuesta)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}
