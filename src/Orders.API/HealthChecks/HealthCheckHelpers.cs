using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace orders.API.HealthChecks;

public static class HealthCheckHelpers
{
    public static Task Escribir(HttpContext context, HealthReport reporte)
    {
        context.Response.ContentType = "application/json";

        Dictionary<string, string> detalle = new Dictionary<string, string>();
        foreach (KeyValuePair<string, HealthReportEntry> entrada in reporte.Entries)
        {
            detalle[entrada.Key] = entrada.Value.Status.ToString();
        }

        var respuesta = new
        {
            status = reporte.Status.ToString(),
            servicio = "orders.API",
            checks = detalle
        };

        string json = JsonSerializer.Serialize(respuesta);
        return context.Response.WriteAsync(json);
    }

    public static bool SoloLive(HealthCheckRegistration registro)
    {
        return registro.Tags.Contains("live");
    }

    public static bool SoloReady(HealthCheckRegistration registro)
    {
        return registro.Tags.Contains("ready");
    }
}