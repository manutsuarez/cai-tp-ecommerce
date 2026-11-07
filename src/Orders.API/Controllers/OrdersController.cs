using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;
using Orders.API.Services;

namespace Orders.API.Controllers;

/// <summary>
/// Creación y seguimiento de órdenes de compra.
/// </summary>
/// <remarks>
/// Datos de prueba: la orden de María (f1e2d3c4-0000-0000-0000-aabbccddeeff), en estado Pendiente.
/// Para probar el error 500 (ORD-007) pedir la orden ffffffff-ffff-ffff-ffff-ffffffffffff (falla simulada).
/// Para crear una orden tienen que estar levantados Users.API y Products.API:
/// se valida el usuario (ORD-003), cada producto (ORD-004) y el stock (ORD-005).
/// </remarks>
[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Lista las órdenes. Se puede filtrar por usuario.
    /// </summary>
    /// <param name="usuarioId" example="a1b2c3d4-0000-0000-0000-111122223333">Filtra por ID de usuario (opcional).</param>
    /// <response code="200">Lista de órdenes.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerOrdenes([FromQuery] Guid? usuarioId)
    {
        List<Order> ordenes = await _orderService.ListarAsync(usuarioId);
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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerOrden(Guid id)
    {
        Order orden = await _orderService.ObtenerPorIdAsync(id);
        return Ok(orden);
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
    /// <response code="422">ORD-005: Stock insuficiente para uno o más productos.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrearOrden([FromBody] CreateOrderRequest request)
    {
        Order orden = await _orderService.CrearAsync(request);
        return Created("/api/orders/" + orden.Id, orden);
    }

    /// <summary>
    /// Actualiza el estado de una orden.
    /// </summary>
    /// <remarks>
    /// Estados posibles: Pendiente, Confirmada, Enviada, Entregada, Cancelada.
    /// Transiciones permitidas: Pendiente → Confirmada → Enviada → Entregada;
    /// Cancelada solo desde Pendiente o Confirmada. Entregada y Cancelada son finales.
    /// </remarks>
    /// <param name="id" example="f1e2d3c4-0000-0000-0000-aabbccddeeff">ID de la orden.</param>
    /// <param name="request">Nuevo estado.</param>
    /// <response code="200">Estado actualizado.</response>
    /// <response code="400">ORD-002: El estado no existe.</response>
    /// <response code="404">ORD-001: Orden no encontrada.</response>
    /// <response code="409">ORD-006: El estado de la orden no puede ser modificado.</response>
    /// <response code="500">ORD-007: Error interno al procesar la orden.</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarEstado(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        OrderStatusResponse respuesta = await _orderService.ActualizarEstadoAsync(id, request.Estado);
        return Ok(respuesta);
    }

    /// <summary>
    /// Lista las órdenes que contienen un producto, con su estado. Uso interno entre servicios
    /// (Products lo usa para PRD-004): no forma parte del contrato público, por eso no aparece en Swagger.
    /// </summary>
    /// <param name="productoId">ID del producto.</param>
    /// <response code="200">Lista de órdenes (vacía si ninguna tiene ese producto).</response>
    [HttpGet("internal/by-product/{productoId:guid}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ListarPorProducto(Guid productoId)
    {
        List<OrderByProductResponse> ordenes = await _orderService.ListarPorProductoAsync(productoId);
        return Ok(ordenes);
    }
}