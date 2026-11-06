using Products.API.Data;
using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

/// <summary>
/// Lógica de negocio de productos: busca, crea, actualiza y elimina.
/// Cuando algo falla lanza la excepción de dominio con el código del catálogo (PRD-00X);
/// no devuelve códigos HTTP, eso lo hace el handler de excepciones.
/// </summary>
public class ProductService : IProductService
{
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository)
    {
        _repository = repository;
    }

    // ── LISTAR ────────────────────────────────────────────────────────────
    public async Task<List<Product>> ListarAsync(string? categoria, string? nombre)
    {
        // Un filtro vacío (por ejemplo el que manda Swagger) es lo mismo que no filtrar
        string? categoriaLimpia = LimpiarFiltro(categoria);
        string? nombreLimpio = LimpiarFiltro(nombre);

        return await _repository.ListarAsync(categoriaLimpia, nombreLimpio);
    }

    // ── OBTENER POR ID ────────────────────────────────────────────────────
    public async Task<Product> ObtenerPorIdAsync(Guid id)
    {
        SimularErrorInterno(id);

        Product? producto = await _repository.ObtenerPorIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");
        }

        return producto;
    }

    // ── CREAR ─────────────────────────────────────────────────────────────
    // Los datos inválidos (PRD-002) ya los frena la validación del pedido (Data Annotations)
    public async Task<Product> CrearAsync(ProductRequest request)
    {
        string nombre = request.Nombre.Trim();
        string categoria = request.Categoria.Trim();

        bool yaExiste = await _repository.ExistePorNombreYCategoriaAsync(nombre, categoria);
        if (yaExiste)
        {
            throw new BusinessRuleException(ErrorCodes.PRD_003,
                "Ya existe un producto con ese nombre en la categoría '" + categoria + "'.");
        }

        Product producto = new Product
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Descripcion = LimpiarFiltro(request.Descripcion),
            Precio = request.Precio,
            Stock = request.Stock,
            Categoria = categoria,
            FechaCreacion = DateTime.UtcNow
        };

        await _repository.InsertarAsync(producto);

        return producto;
    }

    // ── ACTUALIZAR ────────────────────────────────────────────────────────
    public async Task<Product> ActualizarAsync(Guid id, ProductRequest request)
    {
        Product? producto = await _repository.ObtenerPorIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");
        }

        producto.Nombre = request.Nombre.Trim();
        producto.Descripcion = LimpiarFiltro(request.Descripcion);
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        producto.Categoria = request.Categoria.Trim();

        await _repository.ActualizarAsync(producto);

        return producto;
    }

    // ── ELIMINAR ──────────────────────────────────────────────────────────
    public async Task EliminarAsync(Guid id)
    {
        Product? producto = await _repository.ObtenerPorIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException(ErrorCodes.PRD_001, "Producto no encontrado.");
        }

        // PRD-004 (el producto tiene órdenes activas) se agrega en el siguiente PR,
        // cuando Orders tenga su endpoint interno para consultarlo.

        await _repository.EliminarAsync(id);
    }

    // ── Métodos de apoyo ──────────────────────────────────────────────────

    // Devuelve null si el texto está vacío; si no, el texto sin espacios de los costados
    private static string? LimpiarFiltro(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        return texto.Trim();
    }

    // Prueba del error 500 (PRD-005): se mantiene para la demo, documentado en el README
    private static void SimularErrorInterno(Guid id)
    {
        if (id == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}
