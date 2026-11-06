using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

/// <summary>
/// Registro y envío simulado de notificaciones a los usuarios.
/// </summary>
/// <remarks>
/// Datos de prueba: la notificación del enunciado para María (usuario a1b2c3d4-0000-0000-0000-111122223333).
/// Para enviar hace falta que Users.API esté corriendo, porque se verifica que el usuario exista.
/// Para probar errores: enviar con un usuarioId que no existe → NTF-001; tipo distinto de Email, Push o SMS → NTF-002;
/// listar un usuario sin notificaciones → NTF-003; listar ffffffff-ffff-ffff-ffff-ffffffffffff → NTF-004 (falla simulada).
/// </remarks>
[ApiController]
[Route("api/notifications")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Registra una notificación y simula su envío.
    /// </summary>
    /// <param name="pedido">Destinatario, mensaje y tipo de notificación.</param>
    /// <response code="201">Notificación registrada y enviada.</response>
    /// <response code="400">NTF-002: Los datos de la notificación son inválidos.</response>
    /// <response code="404">NTF-001: Usuario no encontrado.</response>
    /// <response code="500">NTF-004: Error interno al procesar la notificación.</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Enviar(EnviarNotificacionRequest pedido)
    {
        Notification notificacion = await _notificationService.EnviarAsync(pedido);
        return StatusCode(StatusCodes.Status201Created, notificacion);
    }

    /// <summary>
    /// Lista las notificaciones de un usuario.
    /// </summary>
    /// <param name="userId" example="a1b2c3d4-0000-0000-0000-111122223333">ID del usuario.</param>
    /// <response code="200">Lista de notificaciones del usuario.</response>
    /// <response code="404">NTF-003: No se encontraron notificaciones para el usuario.</response>
    /// <response code="500">NTF-004: Error interno al procesar la notificación.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarPorUsuario(Guid userId)
    {
        List<Notification> notificaciones = await _notificationService.ListarPorUsuarioAsync(userId);
        return Ok(notificaciones);
    }
}
