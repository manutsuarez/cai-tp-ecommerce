using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    // GET /api/orders?usuarioId=...  -> lista de órdenes (respuesta fija)
    [HttpGet]
    public IActionResult ObtenerOrdenes([FromQuery] Guid? usuarioId)
    {
        List<Order> ordenes = new List<Order>();
        ordenes.Add(CrearOrdenDeEjemplo());
        return Ok(ordenes);
    }

    // GET /api/orders/{id}  -> detalle de una orden (respuesta fija)
    [HttpGet("{id}")]
    public IActionResult ObtenerOrden(Guid id)
    {
        return Ok(CrearOrdenDeEjemplo());
    }

    // POST /api/orders  -> crea una orden (respuesta fija, status 201)
    [HttpPost]
    public IActionResult CrearOrden([FromBody] CreateOrderRequest request)
    {
        Order orden = CrearOrdenDeEjemplo();
        return Created("/api/orders/" + orden.Id, orden);
    }

    // PUT /api/orders/{id}/status  -> actualiza el estado (respuesta fija)
    [HttpPut("{id}/status")]
    public IActionResult ActualizarEstado(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var respuesta = new
        {
            id = id,
            estado = request.Estado,
            fechaActualizacion = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc)
        };
        return Ok(respuesta);
    }

    // Arma una orden fija para devolver en los endpoints de prueba
    // (valores copiados del ejemplo de la sección 4.3 del enunciado)
    private static Order CrearOrdenDeEjemplo()
    {
        OrderItem item = new OrderItem
        {
            ProductoId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Cantidad = 2,
            PrecioUnitario = 1500.00m
        };

        Order orden = new Order
        {
            Id = Guid.Parse("f1e2d3c4-0000-0000-0000-aabbccddeeff"),
            UsuarioId = Guid.Parse("a1b2c3d4-0000-0000-0000-111122223333"),
            Total = 3000.00m,
            Estado = "Pendiente",
            FechaCreacion = new DateTime(2024, 3, 10, 11, 0, 0, DateTimeKind.Utc)
        };
        orden.Items.Add(item);
        return orden;
    }
}