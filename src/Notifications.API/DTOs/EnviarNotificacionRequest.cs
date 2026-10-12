using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "El mensaje es obligatorio.")]
    [StringLength(500, ErrorMessage = "El mensaje no puede superar los 500 caracteres.")]
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>Canal de envío: Email, Push o SMS.</summary>
    /// <example>Email</example>
    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [RegularExpression("^(Email|Push|SMS)$", ErrorMessage = "El tipo debe ser Email, Push o SMS.")]
    public string Tipo { get; set; } = string.Empty;
}