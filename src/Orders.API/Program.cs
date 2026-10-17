using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Orders.API.HealthChecks;
using Orders.API.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

// Serilog se configura primero, antes que cualquier otra cosa.
// Consola: formato legible. Archivo: JSON estructurado (carpeta logs/).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Servicio", "Orders.API")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Servicio:l}] [{CorrelationId:l}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new JsonFormatter(renderMessage: true), "logs/orders-.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger: arma la documentación leyendo los controladores y los comentarios ///
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = typeof(Program).Assembly.GetName().Name + ".xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

// Health checks: el servicio informa si está vivo y listo
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api", tags: new[] { "live", "ready" });

var app = builder.Build();

// Correlation ID y log de cada request (con su duración).
// IMPORTANTE: el UseExceptionHandler() de los errores tiene que ir DESPUÉS de estas dos líneas.
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

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