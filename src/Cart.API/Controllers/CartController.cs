using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    // GET /api/cart/{userId}
    [HttpGet("{userId}")]
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

    // POST /api/cart/{userId}/items
    [HttpPost("{userId}/items")]
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

    // PUT /api/cart/{userId}/items/{productId}
    [HttpPut("{userId}/items/{productId}")]
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

    // DELETE /api/cart/{userId}/items/{productId}
    [HttpDelete("{userId}/items/{productId}")]
    public IActionResult RemoveItem(Guid userId, Guid productId)
    {
        return NoContent();
    }

    // DELETE /api/cart/{userId}
    [HttpDelete("{userId}")]
    public IActionResult ClearCart(Guid userId)
    {
        return NoContent();
    }
}