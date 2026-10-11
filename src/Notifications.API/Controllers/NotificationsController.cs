using Microsoft.AspNetCore.Mvc;
using Notifications.API.Models;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    [HttpPost("send")]
    public IActionResult Enviar(EnviarNotificacionRequest pedido)
    {
        Notification notificacion = new Notification();
        notificacion.Id = Guid.Parse("11112222-3333-4444-5555-666677778888");
        notificacion.UsuarioId = pedido.UsuarioId;
        notificacion.Mensaje = pedido.Mensaje;
        notificacion.Tipo = pedido.Tipo;
        notificacion.Estado = "Enviada";
        notificacion.FechaEnvio = new DateTime(2024, 3, 10, 12, 1, 0, DateTimeKind.Utc);

        return StatusCode(StatusCodes.Status201Created, notificacion);
    }

    [HttpGet("{userId}")]
    public IActionResult ListarPorUsuario(Guid userId)
    {
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
}