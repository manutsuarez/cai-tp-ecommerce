using System.Net;
using System.Net.Http.Json;
using Orders.API.DTOs;
using Serilog;

namespace Orders.API.Clients;

public class ProductsClient
{
    private readonly HttpClient _httpClient;

    public ProductsClient(HttpClient httpClient, IConfiguration configuracion)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuracion["Servicios:ProductsBaseUrl"]!);
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
            Log.Warning("No se pudo contactar a Products.API: {Motivo}", ex.Message);
            return false;
        }
    }

    public async Task<ProductResponse?> ObtenerProductoAsync(Guid productoId)
    {
        HttpResponseMessage respuesta = await _httpClient.GetAsync("/api/products/" + productoId);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<ProductResponse>();
    }
}