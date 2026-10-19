using System.ComponentModel.DataAnnotations;

namespace Cart.API.DTOs;

/// <summary>
/// Producto y cantidad para agregar al carrito.
/// </summary>
public class AddCartItemRequest
{
    /// <summary>ID del producto a agregar.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>Cantidad a agregar. Debe ser mayor a 0.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad inválida. Debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}