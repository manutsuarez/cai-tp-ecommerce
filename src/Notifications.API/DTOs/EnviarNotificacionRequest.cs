namespace Notifications.API.DTOs;

/// <summary>
/// Datos para enviar una notificación.
/// </summary>
public class EnviarNotificacionRequest
{
    /// <summary>ID del usuario destinatario.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Texto de la notificación. Requerido, máximo 500 caracteres.</summary>
    /// <example>Su orden #f1e2d3c4 fue confirmada.</example>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>Canal de envío: Email, Push o SMS.</summary>
    /// <example>Email</example>
    public string Tipo { get; set; } = string.Empty;
}