using Notifications.API.DTOs;
using Notifications.API.Models;

namespace Notifications.API.Services;

/// <summary>
/// Lógica de negocio de notificaciones. El controller solo llama a estos métodos.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Registra una notificación y simula su envío. Lanza NTF-001 si el usuario no existe.
    /// </summary>
    Task<Notification> EnviarAsync(EnviarNotificacionRequest pedido);

    /// <summary>
    /// Lista las notificaciones de un usuario. Lanza NTF-003 si no tiene ninguna.
    /// </summary>
    Task<List<Notification>> ListarPorUsuarioAsync(Guid usuarioId);
}
