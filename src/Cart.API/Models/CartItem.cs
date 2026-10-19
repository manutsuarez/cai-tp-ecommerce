namespace Cart.API.Models;

/// <summary>
/// Un producto dentro del carrito.
/// </summary>
public class CartItem
{
    /// <summary>ID del producto.</summary>
    public Guid ProductoId { get; set; }

    /// <summary>Cantidad de unidades. Requerida, mayor a 0.</summary>
    public int Cantidad { get; set; }
}