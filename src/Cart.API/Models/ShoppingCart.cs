namespace Cart.API.Models;

/// <summary>
/// Carrito de compras de un usuario (el modelo "Cart" del Apéndice A del enunciado).
/// Se llama ShoppingCart porque "Cart" choca con el nombre del proyecto (Cart.API).
/// </summary>
public class ShoppingCart
{
    /// <summary>ID del usuario dueño del carrito.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos que hay en el carrito.</summary>
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    /// <summary>Fecha de la última modificación (UTC). Se actualiza en cada operación.</summary>
    public DateTime FechaActualizacion { get; set; }
}