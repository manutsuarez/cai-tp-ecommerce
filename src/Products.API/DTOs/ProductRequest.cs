namespace Products.API.DTOs;

/// <summary>
/// Datos para crear o actualizar un producto.
/// </summary>
public class ProductRequest
{
    /// <summary>Nombre del producto. Requerido, máximo 100 caracteres.</summary>
    /// <example>Notebook Dell XPS 15</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del producto. Opcional, máximo 500 caracteres.</summary>
    /// <example>Laptop 15 pulgadas, 32GB RAM</example>
    public string? Descripcion { get; set; }

    /// <summary>Precio. Requerido, mayor a 0.</summary>
    /// <example>1500.00</example>
    public decimal Precio { get; set; }

    /// <summary>Stock disponible. Requerido, mayor o igual a 0.</summary>
    /// <example>10</example>
    public int Stock { get; set; }

    /// <summary>Categoría del producto. Requerida.</summary>
    /// <example>Electrónica</example>
    public string Categoria { get; set; } = string.Empty;
}