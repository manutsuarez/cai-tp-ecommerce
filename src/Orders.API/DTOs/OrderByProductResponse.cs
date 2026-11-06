namespace Orders.API.DTOs;

/// <summary>
/// Orden que contiene un producto (respuesta del endpoint interno que usa Products para PRD-004).
/// </summary>
public class OrderByProductResponse
{
    /// <summary>ID de la orden.</summary>
    public Guid Id { get; set; }

    /// <summary>Estado de la orden: Pendiente, Confirmada, Enviada, Entregada o Cancelada.</summary>
    public string Estado { get; set; } = string.Empty;
}