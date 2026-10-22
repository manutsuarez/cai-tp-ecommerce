using System.ComponentModel.DataAnnotations;
namespace Orders.API.DTOs;

/// <summary>
/// Un producto dentro del pedido de creación de una orden.
/// </summary>
public class OrderItemRequest
{
    /// <summary>ID del producto.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>Cantidad de unidades. Debe ser mayor a 0.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}

/// <summary>
/// Datos para crear una orden.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>ID del usuario que hace la orden.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos de la orden. No puede estar vacía.</summary>
    public List<OrderItemRequest> Items { get; set; } = new();
}