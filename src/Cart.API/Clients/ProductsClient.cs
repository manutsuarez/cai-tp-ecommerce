using System.Net;
using System.Net.Http.Json;
using Cart.API.DTOs;
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

    // Trae un producto de Products.API.
    // - 404: el producto no existe → devuelve null (el service lanza CRT-002).
    // - Cualquier otra falla (500, timeout, servicio caído) NO es "no encontrado":
    //   se deja subir la excepción y el handler global responde 500 (CRT-005).
    public async Task<ProductResponse?> ObtenerProductoAsync(Guid productoId)
    {
        HttpResponseMessage respuesta = await _httpClient.GetAsync("/api/products/" + productoId);

        // Primero se mira el status, después se lee el body (lo pide el enunciado)
        if (respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        respuesta.EnsureSuccessStatusCode();

        ProductResponse? producto = await respuesta.Content.ReadFromJsonAsync<ProductResponse>();
        return producto;
    }
}