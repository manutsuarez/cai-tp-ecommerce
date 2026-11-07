using System.Net;
using Serilog;

namespace Orders.API.Clients;

public class UsersClient
{
    private readonly HttpClient _httpClient;

    public UsersClient(HttpClient httpClient, IConfiguration configuracion)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuracion["Servicios:UsersBaseUrl"]!);
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
    }

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

    public async Task<bool> ExisteUsuarioAsync(Guid usuarioId)
    {
        HttpResponseMessage respuesta = await _httpClient.GetAsync("/api/users/" + usuarioId);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        respuesta.EnsureSuccessStatusCode();
        return true;
    }
}