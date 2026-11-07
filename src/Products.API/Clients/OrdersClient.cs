using System.Net.Http.Json;
using Products.API.DTOs;
using Serilog;

namespace Products.API.Clients;

public class OrdersClient
{
    private readonly HttpClient _httpClient;

    public OrdersClient(HttpClient httpClient, IConfiguration configuracion)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuracion["Servicios:OrdersBaseUrl"]!);
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
    }

    // Pregunta a Orders.API si está vivo. Si no responde, devuelve false.
    public async Task<bool> EstaVivoAsync(CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage respuesta = await _httpClient.GetAsync("/health/live", cancellationToken);
            return respuesta.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Warning("No se pudo contactar a Orders.API: {Motivo}", ex.Message);
            return false;
        }
    }

    // Pregunta a Orders.API (endpoint interno) si el producto tiene órdenes activas,
    // es decir, en estado Pendiente o Confirmada.
    // Si Orders.API no responde o falla, NO se asume que "no hay órdenes": se deja subir
    // la excepción y el handler global responde 500 (PRD-005).
    public async Task<bool> TieneOrdenesActivasAsync(Guid productoId)
    {
        HttpResponseMessage respuesta = await _httpClient.GetAsync("/api/orders/internal/by-product/" + productoId);

        // Primero se mira el status, después se lee el body (lo pide el enunciado)
        respuesta.EnsureSuccessStatusCode();

        List<OrdenPorProductoResponse>? ordenes = await respuesta.Content.ReadFromJsonAsync<List<OrdenPorProductoResponse>>();
        if (ordenes == null)
        {
            return false;
        }

        foreach (OrdenPorProductoResponse orden in ordenes)
        {
            if (orden.Estado == "Pendiente" || orden.Estado == "Confirmada")
            {
                return true;
            }
        }

        return false;
    }
}
