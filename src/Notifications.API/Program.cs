using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notifications.API.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

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