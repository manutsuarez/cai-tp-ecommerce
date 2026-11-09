using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;
using Cart.API.Services;

namespace Cart.API.Controllers;

/// <summary>
/// Gestión del carrito de compras de cada usuario.
/// </summary>
/// <remarks>
/// Datos de prueba: el carrito de María (a1b2c3d4-0000-0000-0000-111122223333) con la Notebook × 1
/// y la Silla × 3. Errores: usuario sin carrito, o quitar un producto que no está en el carrito → CRT-001;
/// producto inexistente (o, en el PUT, que no está en el carrito) → CRT-002; cantidad que supera el stock
/// (contando lo que ya hay) → CRT-003; cantidad 0 o negativa → CRT-004;
/// GET con userId ffffffff-ffff-ffff-ffff-ffffffffffff → CRT-005 (falla simulada).
/// </remarks>
[ApiController]
[Route("api/cart")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Obtiene el carrito de un usuario.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <response code="200">Carrito encontrado.</response>
    /// <response code="404">CRT-001: Carrito no encontrado.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        CartResponse response = await _cartService.ObtenerAsync(userId);
        return Ok(response);
    }

    /// <summary>
    /// Agrega un producto al carrito del usuario. Si el producto ya estaba, suma la cantidad.
    /// Si el usuario no tenía carrito, lo crea.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <param name="request">Producto y cantidad a agregar.</param>
    /// <response code="200">Producto agregado. Devuelve el carrito completo.</response>
    /// <response code="400">CRT-004: Cantidad inválida.</response>
    /// <response code="404">CRT-002: Producto no encontrado.</response>
    /// <response code="422">CRT-003: Stock insuficiente para agregar al carrito.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpPost("{userId}/items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem(Guid userId, AddCartItemRequest request)
    {
        CartResponse response = await _cartService.AgregarItemAsync(userId, request);
        return Ok(response);
    }

    /// <summary>
    /// Actualiza (reemplaza) la cantidad de un producto del carrito.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <param name="productId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">ID del producto a modificar.</param>
    /// <param name="request">Nueva cantidad.</param>
    /// <response code="200">Cantidad actualizada. Devuelve el carrito completo.</response>
    /// <response code="400">CRT-004: Cantidad inválida.</response>
    /// <response code="404">CRT-001: Carrito no encontrado. / CRT-002: Producto no encontrado.</response>
    /// <response code="422">CRT-003: Stock insuficiente para agregar al carrito.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpPut("{userId}/items/{productId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateItem(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        CartResponse response = await _cartService.ActualizarItemAsync(userId, productId, request);
        return Ok(response);
    }

    /// <summary>
    /// Quita un producto del carrito.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <param name="productId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">ID del producto a quitar.</param>
    /// <response code="204">Producto quitado. Sin contenido.</response>
    /// <response code="404">CRT-001: Carrito no encontrado.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpDelete("{userId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
    {
        await _cartService.QuitarItemAsync(userId, productId);
        return NoContent();
    }

    /// <summary>
    /// Vacía el carrito completo del usuario.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <response code="204">Carrito vaciado. Sin contenido.</response>
    /// <response code="404">CRT-001: Carrito no encontrado.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        await _cartService.VaciarAsync(userId);
        return NoContent();
    }
}