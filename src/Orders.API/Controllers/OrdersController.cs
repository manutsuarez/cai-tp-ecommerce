using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Controllers;

/// <summary>
/// Creación y seguimiento de órdenes de compra.
/// </summary>
[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    /// <summary>
    /// Lista las órdenes. Se puede filtrar por usuario.
    /// </summary>
    /// <param name="usuarioId" example="a1b2c3d4-0000-0000-0000-111122223333">Filtra por ID de usuario (opcional).</param>
    /// <response code="200">Lista de órdenes.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObtenerOrdenes([FromQuery] Guid? usuarioId)
    {
        List<Order> ordenes = new List<Order>();
        ordenes.Add(CrearOrdenDeEjemplo());
        return Ok(ordenes);
    }

    /// <summary>
    /// Obtiene el detalle de una orden.
    /// </summary>
    /// <param name="id" example="f1e2d3c4-0000-0000-0000-aabbccddeeff">ID de la orden.</param>
    /// <response code="200">Orden encontrada.</response>
    /// <response code="404">ORD-001: Orden no encontrada.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObtenerOrden(Guid id)
    {
        return Ok(CrearOrdenDeEjemplo());
    }

    /// <summary>
    /// Crea una orden nueva.
    /// </summary>
    /// <remarks>
    /// Valida que el usuario exista (Users API), que los productos existan y que haya stock (Products API).
    /// El precio unitario se toma del producto y el total se calcula automáticamente.
    /// </remarks>
    /// <param name="request">Usuario y productos de la orden.</param>
    /// <response code="201">Orden creada en estado Pendiente.</response>
    /// <response code="400">ORD-002: Los datos de la orden son inválidos.</response>
    /// <response code="404">ORD-003: Usuario no encontrado al crear la orden. / ORD-004: Producto no encontrado al crear la orden.</response>
    /// <response code="409">Conflicto al crear la orden (el enunciado no le asigna errorCode).</response>
    /// <response code="422">ORD-005: Stock insuficiente para uno o más productos.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CrearOrden([FromBody] CreateOrderRequest request)
    {
        Order orden = CrearOrdenDeEjemplo();
        return Created("/api/orders/" + orden.Id, orden);
    }

    /// <summary>
    /// Actualiza el estado de una orden.
    /// </summary>
    /// <remarks>
    /// Estados posibles: Pendiente, Confirmada, Enviada, Entregada, Cancelada.
    /// </remarks>
    /// <param name="id" example="f1e2d3c4-0000-0000-0000-aabbccddeeff">ID de la orden.</param>
    /// <param name="request">Nuevo estado.</param>
    /// <response code="200">Estado actualizado.</response>
    /// <response code="400">ORD-002: Los datos de la orden son inválidos.</response>
    /// <response code="404">ORD-001: Orden no encontrada.</response>
    /// <response code="409">ORD-006: El estado de la orden no puede ser modificado.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarEstado(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        OrderStatusResponse respuesta = new OrderStatusResponse
        {
            Id = id,
            Estado = request.Estado,
            FechaActualizacion = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc)
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