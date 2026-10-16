using Serilog;
using Serilog.Context;

namespace Cart.API.Logging;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = context.Request.Headers[HeaderName].ToString();
        if (string.IsNullOrWhiteSpace(correlationId) || correlationId.Length > 100)
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[HeaderName] = correlationId;
        context.Response.OnStarting(AgregarHeaderEnRespuesta, context);

        string endpoint = context.Request.Method + " " + context.Request.Path.Value;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("Endpoint", endpoint))
        {


            {
                Log.Information("Inicio de request {Metodo} {Ruta}", context.Request.Method, context.Request.Path.Value);
                await _next(context);
            }
        }
    }

    private static Task AgregarHeaderEnRespuesta(object estado)
    {
        HttpContext contexto = (HttpContext)estado;
        string correlationId = (string)contexto.Items[HeaderName]!;
        contexto.Response.Headers[HeaderName] = correlationId;
        return Task.CompletedTask;
    }
}