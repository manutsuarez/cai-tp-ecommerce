namespace Orders.API.DTOs;

/// <summary>
/// Datos del producto que Orders lee de Products.API al crear una orden
/// (solo los campos que necesita: nombre, precio y stock).
/// </summary>
public class ProductResponse
{
    /// <summary>ID del producto.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del producto (se usa en el mensaje de ORD-005).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Precio actual: se guarda como PrecioUnitario en la orden.</summary>
    public decimal Precio { get; set; }

    /// <summary>Stock disponible.</summary>
    public int Stock { get; set; }
}