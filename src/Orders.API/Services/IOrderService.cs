using Orders.API.DTOs;

namespace Orders.API.Services;

/// <summary>
/// Lógica de negocio de órdenes. El controller solo llama a estos métodos.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Lista las órdenes que contienen un producto, con su estado (uso interno, PRD-004 de Products).
    /// </summary>
    Task<List<OrderByProductResponse>> ListarPorProductoAsync(Guid productoId);
}