namespace Cart.API.Models;

/// <summary>
/// Carrito de compras de un usuario.
/// </summary>
public class Cart
{
    /// <summary>ID del usuario dueño del carrito.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos que hay en el carrito.</summary>
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    /// <summary>Fecha de la última modificación (UTC). Se actualiza en cada operación.</summary>
    public DateTime FechaActualizacion { get; set; }
}