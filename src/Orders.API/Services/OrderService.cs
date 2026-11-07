using Orders.API.Data;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Models;

namespace Orders.API.Services;

/// <summary>
/// Lógica de negocio de órdenes. Cuando algo falla lanza la excepción de dominio
/// con el código del catálogo (ORD-00X); no devuelve códigos HTTP.
/// </summary>
public class OrderService : IOrderService
{
    private const string Pendiente = "Pendiente";
    private const string Confirmada = "Confirmada";
    private const string Enviada = "Enviada";
    private const string Entregada = "Entregada";
    private const string Cancelada = "Cancelada";

    private readonly OrderRepository _repository;

    public OrderService(OrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Order>> ListarAsync(Guid? usuarioId)
    {
        return await _repository.ListarAsync(usuarioId);
    }

    public async Task<Order> ObtenerPorIdAsync(Guid id)
    {
        SimularErrorInterno(id);

        Order? orden = await _repository.ObtenerPorIdAsync(id);
        if (orden == null)
        {
            throw new NotFoundException(ErrorCodes.ORD_001, "Orden no encontrada.");
        }

        return orden;
    }

    public async Task<OrderStatusResponse> ActualizarEstadoAsync(Guid id, string estado)
    {
        string estadoNuevo = string.Empty;
        if (estado != null)
        {
            estadoNuevo = estado.Trim();
        }

        if (!EsEstadoValido(estadoNuevo))
        {
            throw new ValidationException(ErrorCodes.ORD_002,
                "El estado '" + estadoNuevo + "' no existe. Estados posibles: Pendiente, Confirmada, Enviada, Entregada o Cancelada.");
        }

        Order? orden = await _repository.ObtenerPorIdAsync(id);
        if (orden == null)
        {
            throw new NotFoundException(ErrorCodes.ORD_001, "Orden no encontrada.");
        }

        if (!EsTransicionValida(orden.Estado, estadoNuevo))
        {
            throw new BusinessRuleException(ErrorCodes.ORD_006,
                "La orden no puede pasar de '" + orden.Estado + "' a '" + estadoNuevo + "'.");
        }

        DateTime fechaActualizacion = DateTime.UtcNow;
        await _repository.ActualizarEstadoAsync(id, estadoNuevo, fechaActualizacion);

        OrderStatusResponse respuesta = new OrderStatusResponse
        {
            Id = id,
            Estado = estadoNuevo,
            FechaActualizacion = fechaActualizacion
        };
        return respuesta;
    }

    public async Task<List<OrderByProductResponse>> ListarPorProductoAsync(Guid productoId)
    {
        return await _repository.ListarPorProductoAsync(productoId);
    }

    private static bool EsEstadoValido(string estado)
    {
        List<string> estados = new List<string>();
        estados.Add(Pendiente);
        estados.Add(Confirmada);
        estados.Add(Enviada);
        estados.Add(Entregada);
        estados.Add(Cancelada);

        return estados.Contains(estado);
    }

    private static bool EsTransicionValida(string actual, string nuevo)
    {
        if (actual == Pendiente)
        {
            return nuevo == Confirmada || nuevo == Cancelada;
        }

        if (actual == Confirmada)
        {
            return nuevo == Enviada || nuevo == Cancelada;
        }

        if (actual == Enviada)
        {
            return nuevo == Entregada;
        }

        return false;
    }

    private static void SimularErrorInterno(Guid id)
    {
        if (id == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}