using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Controllers;

/// <summary>
/// Registro y envío simulado de notificaciones a los usuarios.
/// </summary>
/// <remarks>
/// Datos para probar errores (respuestas de prueba): enviar con usuarioId todo ceros → NTF-001;
/// listar con userId todo ceros → NTF-003; tipo distinto de Email, Push o SMS → NTF-002;
/// listar con userId ffffffff-ffff-ffff-ffff-ffffffffffff → NTF-004 (falla simulada).
/// </remarks>
[ApiController]
[Route("api/notifications")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
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
    public IActionResult Enviar(EnviarNotificacionRequest pedido)
    {
        ValidarUsuarioExiste(pedido.UsuarioId);

        Notification notificacion = new Notification();
        notificacion.Id = Guid.Parse("11112222-3333-4444-5555-666677778888");
        notificacion.UsuarioId = pedido.UsuarioId;
        notificacion.Mensaje = pedido.Mensaje;
        notificacion.Tipo = pedido.Tipo;
        notificacion.Estado = "Enviada";
        notificacion.FechaEnvio = new DateTime(2024, 3, 10, 12, 1, 0, DateTimeKind.Utc);

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
    public IActionResult ListarPorUsuario(Guid userId)
    {
        SimularErrorInterno(userId);
        ValidarTieneNotificaciones(userId);

        Notification notificacion = new Notification();
        notificacion.Id = Guid.Parse("11112222-3333-4444-5555-666677778888");
        notificacion.UsuarioId = userId;
        notificacion.Mensaje = "Su orden fue confirmada.";
        notificacion.Tipo = "Email";
        notificacion.Estado = "Enviada";
        notificacion.FechaEnvio = new DateTime(2024, 3, 10, 12, 1, 0, DateTimeKind.Utc);

        List<Notification> lista = new List<Notification>();
        lista.Add(notificacion);
        return Ok(lista);
    }

    // --- Validaciones de prueba (se reemplazan cuando haya lógica real) ---

    private static void ValidarUsuarioExiste(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new NotFoundException(ErrorCodes.NTF_001, "El usuario destinatario no fue encontrado.");
        }
    }

    private static void ValidarTieneNotificaciones(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new NotFoundException(ErrorCodes.NTF_003, "No se encontraron notificaciones para el usuario.");
        }
    }

    private static void SimularErrorInterno(Guid id)
    {
        if (id == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }

}