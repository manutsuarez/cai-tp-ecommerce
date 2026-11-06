using Serilog;

namespace Notifications.API.Clients;

/// <summary>
/// Cliente HTTP para hablar con Users.API.
/// </summary>
public class UsersClient
{
    private readonly HttpClient _httpClient;

    public UsersClient(HttpClient httpClient, IConfiguration configuracion)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuracion["Servicios:UsersBaseUrl"]!);
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
    }

    // Pregunta a Users.API si está vivo. Si no responde, devuelve false.
    public async Task<bool> EstaVivoAsync(CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage respuesta = await _httpClient.GetAsync("/health/live", cancellationToken);
            return respuesta.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Warning("No se pudo contactar a Users.API: {Motivo}", ex.Message);
            return false;
        }
    }

    // Pregunta a Users.API si el usuario existe (endpoint interno GET /api/users/{id}).
    // 200 → true. 404 → false. Cualquier otra cosa (Users caído, demora, error 500) NO se
    // confunde con "no existe": se lanza la excepción y termina en el 500 del manejador global.
    public async Task<bool> ExisteUsuarioAsync(Guid usuarioId)
    {
        HttpResponseMessage respuesta = await _httpClient.GetAsync("/api/users/" + usuarioId);

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        respuesta.EnsureSuccessStatusCode();
        return true;
    }
}
