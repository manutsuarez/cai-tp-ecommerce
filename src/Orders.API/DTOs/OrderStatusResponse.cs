namespace Orders.API.DTOs;

/// <summary>
/// Resultado de actualizar el estado de una orden.
/// </summary>
public class OrderStatusResponse
{
    /// <summary>ID de la orden.</summary>
    /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
    public Guid Id { get; set; }

    /// <summary>Estado actual de la orden.</summary>
    /// <example>Confirmada</example>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Fecha del cambio de estado (UTC).</summary>
    /// <example>2024-03-10T12:00:00Z</example>
    public DateTime FechaActualizacion { get; set; }
}