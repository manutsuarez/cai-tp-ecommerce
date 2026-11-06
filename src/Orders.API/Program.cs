using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.ExceptionHandlers;
using Orders.API.Exceptions;
using Orders.API.HealthChecks;
using Orders.API.Logging;
using Orders.API.Swagger;
using Orders.API.Configuracion;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Orders.API.Clients;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Orders.API.Data;
using Orders.API.Services;
using Dapper;

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

// Respuesta 400 cuando el body no pasa las validaciones (Data Annotations).
// La configuración está en Configuracion/ConfiguracionApi.cs
builder.Services.AddTransient<IConfigureOptions<ApiBehaviorOptions>, ConfiguracionApi>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger: arma la documentación leyendo los controladores y los comentarios ///
// (la configuración está en Swagger/ConfiguracionSwagger.cs)
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfiguracionSwagger>();

// Persistencia: SQLite + Dapper (patrón de la cátedra).
// Los handlers le explican a Dapper cómo leer Guid y fechas guardados como texto.
SqlMapper.AddTypeHandler(new GuidTypeHandler());
SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Llamadas a otros servicios: cada llamada saliente lleva el mismo Correlation ID del request
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdHandler>();
builder.Services.AddHttpClient<UsersClient>()
    .AddHttpMessageHandler<CorrelationIdHandler>();
builder.Services.AddHttpClient<ProductsClient>()
    .AddHttpMessageHandler<CorrelationIdHandler>();

// Health checks: el servicio informa si está vivo y listo
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api", tags: new[] { "live", "ready" })
    .AddCheck<UsersHealthCheck>("users", tags: new[] { "ready" })
    .AddCheck<ProductsHealthCheck>("products", tags: new[] { "ready" })
    .AddCheck<SqliteHealthCheck>("sqlite", tags: new[] { "ready" });

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Crear las tablas y cargar los datos de prueba al arrancar
using (IServiceScope scope = app.Services.CreateScope())
{
    DatabaseInitializer inicializador = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    inicializador.Initialize();
}

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