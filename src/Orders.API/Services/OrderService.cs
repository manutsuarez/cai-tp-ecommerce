using Orders.API.Data;
using Orders.API.DTOs;

namespace Orders.API.Services;

/// <summary>
/// Lógica de negocio de órdenes. Cuando algo falla lanza la excepción de dominio
/// con el código del catálogo (ORD-00X); no devuelve códigos HTTP.
/// </summary>
public class OrderService : IOrderService
{
    private readonly OrderRepository _repository;

    public OrderService(OrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderByProductResponse>> ListarPorProductoAsync(Guid productoId)
    {
        return await _repository.ListarPorProductoAsync(productoId);
    }
}