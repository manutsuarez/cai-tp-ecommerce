namespace Cart.API.DTOs;

/// <summary>
/// Nueva cantidad para un producto del carrito.
/// </summary>
public class UpdateCartItemRequest
{
    /// <summary>Nueva cantidad. Debe ser mayor a 0.</summary>
    /// <example>4</example>
    public int Cantidad { get; set; }
}