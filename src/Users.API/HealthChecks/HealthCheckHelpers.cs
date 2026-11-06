using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Users.API.HealthChecks;

public static class HealthCheckHelpers
{
    public static Task Escribir(HttpContext context, HealthReport reporte)
    {
        context.Response.ContentType = "application/json";

        // El JSON se arma a mano con un StringBuilder (sin diccionarios):
        // {"status":"Healthy","servicio":"Users.API","checks":{"api":"Healthy"}}
        StringBuilder checks = new StringBuilder();
        bool esElPrimero = true;
        foreach (KeyValuePair<string, HealthReportEntry> entrada in reporte.Entries)
        {
            if (!esElPrimero)
            {
                checks.Append(",");
            }

            checks.Append("\"" + entrada.Key + "\":\"" + entrada.Value.Status + "\"");
            esElPrimero = false;
        }

        string json = "{\"status\":\"" + reporte.Status + "\",\"servicio\":\"Users.API\",\"checks\":{" + checks + "}}";
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