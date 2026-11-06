namespace Cart.API.DTOs;

/// <summary>
/// Producto tal como lo devuelve Products.API (GET /api/products/{id}).
/// Cart solo necesita estos tres datos: para saber que existe, su nombre y su stock.
/// </summary>
public class ProductResponse
{
    /// <summary>ID del producto.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del producto (se usa en el mensaje de stock insuficiente).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Stock disponible en Products.</summary>
    public int Stock { get; set; }
}