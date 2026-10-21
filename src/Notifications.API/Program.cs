using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.ExceptionHandlers;
using Notifications.API.Exceptions;
using Notifications.API.HealthChecks;
using Notifications.API.Logging;
using Notifications.API.Swagger; // NUEVO
using Notifications.API.Clients;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

// Serilog se configura primero, antes que cualquier otra cosa.
// Consola: formato legible. Archivo: JSON estructurado (carpeta logs/).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Servicio", "Notifications.API")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Servicio:l}] [{CorrelationId:l}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new JsonFormatter(renderMessage: true), "logs/notifications-.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            List<string> mensajes = new List<string>();
            foreach (var entrada in context.ModelState.Values)
            {
                foreach (var error in entrada.Errors)
                {
                    mensajes.Add(error.ErrorMessage);
                }
            }

            Log.Warning("Datos inválidos. ErrorCode {ErrorCode}: {ErrorMessage}", ErrorCodes.NTF_002, string.Join("; ", mensajes));

            ErrorResponse respuesta = new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Los datos enviados son inválidos.",
                Instance = context.HttpContext.Request.Path.Value,
                ErrorCode = ErrorCodes.NTF_002,
                ErrorMessage = string.Join("; ", mensajes),
                CorrelationId = context.HttpContext.Items["X-Correlation-Id"] as string
            };

            return new ObjectResult(respuesta)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger: arma la documentación leyendo los controladores y los comentarios ///
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = typeof(Program).Assembly.GetName().Name + ".xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    options.OperationFilter<EjemplosDeErrorFilter>(); // NUEVO: ejemplos de error por código
});

// Llamadas a otros servicios: cada llamada saliente lleva el mismo Correlation ID del request
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdHandler>();
builder.Services.AddHttpClient<UsersClient>()
    .AddHttpMessageHandler<CorrelationIdHandler>();

// Health checks: el servicio informa si está vivo y listo
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api", tags: new[] { "live", "ready" })
    .AddCheck<UsersHealthCheck>("users", tags: new[] { "ready" });

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Correlation ID y log de cada request (con su duración).
// IMPORTANTE: el UseExceptionHandler() de los errores tiene que ir DESPUÉS de estas dos líneas.
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckHelpers.Escribir
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = HealthCheckHelpers.SoloReady,
    ResponseWriter = HealthCheckHelpers.Escribir
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = HealthCheckHelpers.SoloLive,
    ResponseWriter = HealthCheckHelpers.Escribir
});

app.Run();