namespace Orders.API.DTOs;

/// <summary>
/// Nuevo estado para una orden.
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>Nuevo estado: Pendiente, Confirmada, Enviada, Entregada o Cancelada.</summary>
    /// <example>Confirmada</example>
    public string Estado { get; set; } = string.Empty;
}