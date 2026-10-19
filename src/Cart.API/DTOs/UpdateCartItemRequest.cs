using System.ComponentModel.DataAnnotations;

namespace Cart.API.DTOs;

/// <summary>
/// Nueva cantidad para un producto del carrito.
/// </summary>
public class UpdateCartItemRequest
{
    /// <summary>Nueva cantidad. Debe ser mayor a 0.</summary>
    /// <example>4</example>
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad inválida. Debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}