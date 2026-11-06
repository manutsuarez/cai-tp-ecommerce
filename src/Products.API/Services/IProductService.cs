using Products.API.DTOs;
using Products.API.Models;

namespace Products.API.Services;

/// <summary>
/// Lógica de negocio de productos. El controller solo llama a estos métodos.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Lista los productos, con filtros opcionales por categoría y por nombre.
    /// </summary>
    Task<List<Product>> ListarAsync(string? categoria, string? nombre);

    /// <summary>
    /// Busca un producto por su ID. Lanza PRD-001 si no existe.
    /// </summary>
    Task<Product> ObtenerPorIdAsync(Guid id);

    /// <summary>
    /// Crea un producto. Lanza PRD-003 si ya existe uno con ese nombre en la categoría.
    /// </summary>
    Task<Product> CrearAsync(ProductRequest request);

    /// <summary>
    /// Actualiza un producto. Lanza PRD-001 si no existe.
    /// </summary>
    Task<Product> ActualizarAsync(Guid id, ProductRequest request);

    /// <summary>
    /// Elimina un producto. Lanza PRD-001 si no existe.
    /// </summary>
    Task EliminarAsync(Guid id);
}
