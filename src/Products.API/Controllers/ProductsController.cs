using Microsoft.AspNetCore.Mvc;
using Products.API.Models;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Listar(string? categoria, string? nombre)
    {
        List<Product> productos = new List<Product>();
        productos.Add(CrearProductoEjemplo());
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(Guid id)
    {
        Product producto = CrearProductoEjemplo();
        producto.Id = id;
        return Ok(producto);
    }

    [HttpPost]
    public IActionResult Crear(Product producto)
    {
        return StatusCode(StatusCodes.Status201Created, CrearProductoEjemplo());
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(Guid id, Product producto)
    {
        Product respuesta = CrearProductoEjemplo();
        respuesta.Id = id;
        return Ok(respuesta);
    }

    [HttpDelete("{id}")]
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