using Cart.API.DTOs;

namespace Cart.API.Services;

/// <summary>
/// Lógica de negocio del carrito. El controller solo llama a estos métodos.
/// </summary>
public interface ICartService
{
    /// <summary>Devuelve el carrito del usuario. Lanza CRT-001 si no tiene carrito.</summary>
    Task<CartResponse> ObtenerAsync(Guid usuarioId);

    /// <summary>
    /// Agrega un producto (suma a lo que ya había). Si el usuario no tenía carrito, lo crea.
    /// Lanza CRT-002, CRT-003 o CRT-004.
    /// </summary>
    Task<CartResponse> AgregarItemAsync(Guid usuarioId, AddCartItemRequest request);

    /// <summary>
    /// Reemplaza la cantidad de un producto del carrito. Lanza CRT-001, CRT-002, CRT-003 o CRT-004.
    /// </summary>
    Task<CartResponse> ActualizarItemAsync(Guid usuarioId, Guid productoId, UpdateCartItemRequest request);

    /// <summary>Quita un producto del carrito. Lanza CRT-001 o CRT-002.</summary>
    Task QuitarItemAsync(Guid usuarioId, Guid productoId);

    /// <summary>Vacía el carrito. Lanza CRT-001.</summary>
    Task VaciarAsync(Guid usuarioId);
}