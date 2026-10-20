using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Products.API.Swagger;

/// <summary>
/// Arma en Swagger un ejemplo de respuesta para cada código de error, usando el texto
/// de los comentarios &lt;response&gt; del controlador (por ejemplo "PRD-001: Producto no encontrado.").
/// </summary>
public class EjemplosDeErrorFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses == null)
        {
            return;
        }

        string ruta = "/" + context.ApiDescription.RelativePath;

        foreach (var par in operation.Responses)
        {
            int status;
            if (!int.TryParse(par.Key, out status) || status < 400)
            {
                continue;
            }

            var respuesta = par.Value;
            if (respuesta.Content == null || string.IsNullOrEmpty(respuesta.Description))
            {
                continue;
            }

            // Si hay dos códigos ("XXX-001: ... / XXX-002: ..."), se usa el primero
            string primero = respuesta.Description.Split(" / ")[0];
            int dosPuntos = primero.IndexOf(':');
            if (dosPuntos < 0)
            {
                continue;
            }

            string errorCode = primero.Substring(0, dosPuntos).Trim();
            string errorMessage = primero.Substring(dosPuntos + 1).Trim();

            JsonObject ejemplo = new JsonObject
            {
                ["type"] = TipoSegunStatus(status),
                ["title"] = TituloSegunStatus(status),
                ["status"] = status,
                ["detail"] = DetalleSegunStatus(status, errorCode),
                ["instance"] = ruta,
                ["errorCode"] = errorCode,
                ["errorMessage"] = errorMessage,
                ["correlationId"] = "138c20e6-22e3-4ca5-8185-ec800103f14b"
            };

            foreach (var media in respuesta.Content.Values)
            {
                media.Example = ejemplo.DeepClone();
            }
        }
    }

    private static string TipoSegunStatus(int status)
    {
        if (status == 400) return "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        if (status == 401) return "https://tools.ietf.org/html/rfc7235#section-3.1";
        if (status == 403) return "https://tools.ietf.org/html/rfc7231#section-6.5.3";
        if (status == 404) return "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        if (status == 409) return "https://tools.ietf.org/html/rfc7231#section-6.5.9";
        if (status == 422) return "https://tools.ietf.org/html/rfc4918#section-11.2";
        return "https://tools.ietf.org/html/rfc7231#section-6.6.1";
    }

    private static string TituloSegunStatus(int status)
    {
        if (status == 400) return "Bad Request";
        if (status == 401) return "Unauthorized";
        if (status == 403) return "Forbidden";
        if (status == 404) return "Not Found";
        if (status == 409) return "Conflict";
        if (status == 422) return "Unprocessable Entity";
        return "Internal Server Error";
    }

    private static string DetalleSegunStatus(int status, string errorCode)
    {
        if (status == 400) return "Los datos enviados son inválidos.";
        if (status == 401) return "Las credenciales no son válidas.";
        if (status == 403) return "El acceso está prohibido.";
        if (status == 404) return "El recurso solicitado no fue encontrado.";
        if (status == 409 && errorCode == "ORD-006") return "No se puede modificar el estado.";
        if (status == 409) return "Ya existe un recurso con esos datos.";
        if (status == 422) return "No se puede procesar la solicitud.";
        return "Ocurrió un error inesperado.";
    }
}