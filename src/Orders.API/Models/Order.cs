namespace Orders.API.Models;

/// <summary>
/// Orden de compra de un usuario.
/// </summary>
public class Order
{
    /// <summary>Identificador único de la orden.</summary>
    /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
    public Guid Id { get; set; }

    /// <summary>ID del usuario que hizo la orden.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos incluidos en la orden.</summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>Total de la orden, calculado automáticamente al crearla.</summary>
    /// <example>3000.00</example>
    public decimal Total { get; set; }

    /// <summary>Estado: Pendiente, Confirmada, Enviada, Entregada o Cancelada.</summary>
    /// <example>Pendiente</example>
    public string Estado { get; set; } = "Pendiente";

    /// <summary>Fecha de creación (UTC), asignada automáticamente.</summary>
    /// <example>2024-03-10T11:00:00Z</example>
    public DateTime FechaCreacion { get; set; }
}