namespace Orders.API.Models;

/// <summary>
/// Un producto dentro de una orden.
/// </summary>
public class OrderItem
{
    /// <summary>ID del producto.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>Cantidad de unidades. Mayor a 0.</summary>
    /// <example>2</example>
    public int Cantidad { get; set; }

    /// <summary>Precio unitario, tomado del producto al crear la orden.</summary>
    /// <example>1500.00</example>
    public decimal PrecioUnitario { get; set; }
}