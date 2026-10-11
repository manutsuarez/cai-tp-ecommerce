using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Models;

namespace Products.API.Controllers;

/// <summary>
/// Catálogo de productos.
/// </summary>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Lista los productos. Se puede filtrar por categoría y por nombre.
    /// </summary>
    /// <param name="categoria" example="Electrónica">Filtra por categoría (opcional).</param>
    /// <param name="nombre" example="Notebook">Filtra por nombre (opcional).</param>
    /// <response code="200">Lista de productos.</response>
    /// <response code="500">PRD-005: Error interno al procesar el producto.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Listar(string? categoria, string? nombre)
    {
        List<Product> productos = new List<Product>();
        productos.Add(CrearProductoEjemplo());
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObtenerPorId(Guid id)
    {
        Product producto = CrearProductoEjemplo();
        producto.Id = id;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Crear(ProductRequest producto)
    {
        Product respuesta = new Product();
        respuesta.Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        respuesta.Nombre = producto.Nombre;
        respuesta.Descripcion = producto.Descripcion;
        respuesta.Precio = producto.Precio;
        respuesta.Stock = producto.Stock;
        respuesta.Categoria = producto.Categoria;
        respuesta.FechaCreacion = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        return StatusCode(StatusCodes.Status201Created, respuesta);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Actualizar(Guid id, ProductRequest producto)
    {
        Product respuesta = new Product();
        respuesta.Id = id;
        respuesta.Nombre = producto.Nombre;
        respuesta.Descripcion = producto.Descripcion;
        respuesta.Precio = producto.Precio;
        respuesta.Stock = producto.Stock;
        respuesta.Categoria = producto.Categoria;
        respuesta.FechaCreacion = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        return Ok(respuesta);
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Eliminar(Guid id)
    {
        return NoContent();
    }

    private Product CrearProductoEjemplo()
    {
        Product producto = new Product();
        producto.Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        producto.Nombre = "Notebook Dell XPS 15";
        producto.Descripcion = "Laptop 15 pulgadas, 32GB RAM";
        producto.Precio = 1500.00m;
        producto.Stock = 10;
        producto.Categoria = "Electrónica";
        producto.FechaCreacion = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        return producto;
    }
}