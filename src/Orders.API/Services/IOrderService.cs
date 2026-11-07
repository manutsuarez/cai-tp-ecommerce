using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Services;

/// <summary>
/// Lógica de negocio de órdenes. El controller solo llama a estos métodos.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Lista las órdenes, con filtro opcional por usuario.
    /// </summary>
    Task<List<Order>> ListarAsync(Guid? usuarioId);

    /// <summary>
    /// Busca una orden por su ID. Lanza ORD-001 si no existe.
    /// </summary>
    Task<Order> ObtenerPorIdAsync(Guid id);

    /// <summary>
    /// Cambia el estado de una orden. Lanza ORD-001 si no existe, ORD-002 si el estado
    /// no existe y ORD-006 si la transición no está permitida.
    /// </summary>
    Task<OrderStatusResponse> ActualizarEstadoAsync(Guid id, string estado);

    /// <summary>
    /// Lista las órdenes que contienen un producto, con su estado (uso interno, PRD-004 de Products).
    /// </summary>
    Task<List<OrderByProductResponse>> ListarPorProductoAsync(Guid productoId);
}