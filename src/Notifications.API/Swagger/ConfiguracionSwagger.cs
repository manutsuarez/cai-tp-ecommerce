using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notifications.API.Swagger;

/// <summary>
/// Configuración de Swagger: incluye los comentarios /// del código
/// y los ejemplos de error por código.
/// </summary>
public class ConfiguracionSwagger : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        string xmlFile = typeof(Program).Assembly.GetName().Name + ".xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
        options.OperationFilter<EjemplosDeErrorFilter>();
    }
}