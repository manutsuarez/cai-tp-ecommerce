namespace Cart.API.DTOs;

/// <summary>
/// Carrito completo de un usuario.
/// </summary>
public class CartResponse
{
    /// <summary>ID del usuario dueño del carrito.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos que hay en el carrito.</summary>
    public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();

    /// <summary>Fecha de la última modificación (UTC).</summary>
    /// <example>2024-03-10T10:45:00Z</example>
    public DateTime FechaActualizacion { get; set; }
}