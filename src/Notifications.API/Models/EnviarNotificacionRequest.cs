namespace Notifications.API.Models;

public class EnviarNotificacionRequest
{
    public Guid UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}