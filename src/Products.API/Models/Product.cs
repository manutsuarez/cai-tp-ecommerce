namespace Products.API.Models;

/// <summary>
/// Producto del catálogo.
/// </summary>
public class Product
{
    /// <summary>Identificador único del producto.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

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

    /// <summary>Fecha de creación (UTC), asignada automáticamente.</summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime FechaCreacion { get; set; }
}