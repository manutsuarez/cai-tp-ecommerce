using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers;

/// <summary>
/// Catálogo de productos.
/// </summary>
/// <remarks>
/// Datos de prueba: la Notebook del enunciado (3fa85f64-5717-4562-b3fc-2c963f66afa6) y otros dos productos.
/// Para probar el error 500 (PRD-005) pedir el producto ffffffff-ffff-ffff-ffff-ffffffffffff (falla simulada).
/// </remarks>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lista los productos. Se puede filtrar por categoría y por nombre.
    /// </summary>
    /// <param name="categoria" example="Electrónica">Filtra por categoría (opcional).</param>
    /// <param name="nombre" example="Notebook">Filtra por nombre (opcional).</param>
    /// <response code="200">Lista de productos.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Listar(string? categoria, string? nombre)
    {
        List<Product> productos = await _productService.ListarAsync(categoria, nombre);
        return Ok(productos);
    }

    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    /// <param name="id" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">ID del producto.</param>
    /// <response code="200">Producto encontrado.</response>
    /// <response code="404">PRD-001: Producto no encontrado.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        Product producto = await _productService.ObtenerPorIdAsync(id);
        return Ok(producto);
    }

    /// <summary>
    /// Crea un producto nuevo.
    /// </summary>
    /// <param name="producto">Datos del producto a crear.</param>
    /// <response code="201">Producto creado.</response>
    /// <response code="400">PRD-002: Los datos del producto son inválidos.</response>
    /// <response code="409">PRD-003: Ya existe un producto con ese nombre en la categoría.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear(ProductRequest producto)
    {
        Product creado = await _productService.CrearAsync(producto);
        return StatusCode(StatusCodes.Status201Created, creado);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    /// <param name="id" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">ID del producto a actualizar.</param>
    /// <param name="producto">Nuevos datos del producto.</param>
    /// <response code="200">Producto actualizado.</response>
    /// <response code="400">PRD-002: Los datos del producto son inválidos.</response>
    /// <response code="404">PRD-001: Producto no encontrado.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(Guid id, ProductRequest producto)
    {
        Product actualizado = await _productService.ActualizarAsync(id, producto);
        return Ok(actualizado);
    }

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    /// <remarks>
    /// No se puede eliminar si tiene órdenes en estado Pendiente o Confirmada.
    /// </remarks>
    /// <param name="id" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">ID del producto a eliminar.</param>
    /// <response code="204">Producto eliminado. Sin contenido.</response>
    /// <response code="404">PRD-001: Producto no encontrado.</response>
    /// <response code="409">PRD-004: El producto tiene órdenes activas y no puede eliminarse.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _productService.EliminarAsync(id);
        return NoContent();
    }
}
