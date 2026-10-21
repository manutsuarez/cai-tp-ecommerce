using Serilog;

namespace Cart.API.Clients;

public class ProductsClient
{
    private readonly HttpClient _httpClient;

    public ProductsClient(HttpClient httpClient, IConfiguration configuracion)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuracion["Servicios:ProductsBaseUrl"]!);
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
    }

    // Pregunta a Products.API si está vivo. Si no responde, devuelve false.
    public async Task<bool> EstaVivoAsync(CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage respuesta = await _httpClient.GetAsync("/health/live", cancellationToken);
            return respuesta.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Warning("No se pudo contactar a Products.API: {Motivo}", ex.Message);
            return false;
        }
    }
}