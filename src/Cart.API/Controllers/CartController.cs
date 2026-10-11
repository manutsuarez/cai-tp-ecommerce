using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;

namespace Cart.API.Controllers;

/// <summary>
/// Gestión del carrito de compras de cada usuario.
/// </summary>
[ApiController]
[Route("api/cart")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    /// <summary>
    /// Obtiene el carrito de un usuario.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario dueño del carrito.</param>
    /// <response code="200">Carrito encontrado.</response>
    /// <response code="404">CRT-001: Carrito no encontrado.</response>
    /// <response code="500">CRT-005: Error interno al procesar el carrito.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCart(Guid userId)
    {
        CartResponse response = new CartResponse
        {
            UsuarioId = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Items = new List<CartItemResponse>
            {
                new CartItemResponse { ProductoId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Cantidad = 1 },
                new CartItemResponse { ProductoId = new Guid("aaaabbbb-cccc-dddd-eeee-ffff00001111"), Cantidad = 3 }
            },
            FechaActualizacion = new DateTime(2024, 3, 10, 10, 45, 0, DateTimeKind.Utc)
        };

        return Ok(response);
    }

    /// <summary>
    /// Agrega un producto al carrito del usuario.
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddItem(Guid userId, AddCartItemRequest request)
    {
        CartResponse response = new CartResponse
        {
            UsuarioId = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Items = new List<CartItemResponse>
            {
                new CartItemResponse { ProductoId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Cantidad = 2 }
            },
            FechaActualizacion = new DateTime(2024, 3, 10, 10, 50, 0, DateTimeKind.Utc)
        };

        return Ok(response);
    }

    /// <summary>
    /// Actualiza la cantidad de un producto del carrito.
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateItem(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        CartResponse response = new CartResponse
        {
            UsuarioId = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Items = new List<CartItemResponse>
            {
                new CartItemResponse { ProductoId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Cantidad = 4 }
            },
            FechaActualizacion = new DateTime(2024, 3, 10, 11, 5, 0, DateTimeKind.Utc)
        };

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RemoveItem(Guid userId, Guid productId)
    {
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ClearCart(Guid userId)
    {
        return NoContent();
    }
}