namespace Orders.API.DTOs;

public class OrderItemRequest
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class CreateOrderRequest
{
    public Guid UsuarioId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}
