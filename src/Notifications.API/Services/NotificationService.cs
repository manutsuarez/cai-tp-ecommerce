using Notifications.API.Clients;
using Notifications.API.Data;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;
using Serilog;

namespace Notifications.API.Services;

/// <summary>
/// Lógica de negocio de notificaciones: valida el usuario, registra y lista.
/// Cuando algo falla lanza la excepción de dominio con el código del catálogo (NTF-00X);
/// no devuelve códigos HTTP, eso lo hace el handler de excepciones.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly NotificationRepository _repository;
    private readonly UsersClient _usersClient;

    public NotificationService(NotificationRepository repository, UsersClient usersClient)
    {
        _repository = repository;
        _usersClient = usersClient;
    }

    // ── ENVIAR ────────────────────────────────────────────────────────────
    // Los datos inválidos (NTF-002) ya los frena la validación del pedido (Data Annotations)
    public async Task<Notification> EnviarAsync(EnviarNotificacionRequest pedido)
    {
        // NTF-001: el usuario destinatario tiene que existir en Users.API
        bool existe = await _usersClient.ExisteUsuarioAsync(pedido.UsuarioId);
        if (!existe)
        {
            throw new NotFoundException(ErrorCodes.NTF_001, "El usuario destinatario no fue encontrado.");
        }

        // El envío es simulado: se registra directamente como Enviada
        Notification notificacion = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = pedido.UsuarioId,
            Mensaje = pedido.Mensaje.Trim(),
            Tipo = pedido.Tipo,
            Estado = "Enviada",
            FechaEnvio = DateTime.UtcNow
        };

        await _repository.InsertarAsync(notificacion);

        Log.Information("Notificación {Tipo} enviada al usuario {UsuarioId}", notificacion.Tipo, notificacion.UsuarioId);

        return notificacion;
    }

    // ── LISTAR POR USUARIO ────────────────────────────────────────────────
    public async Task<List<Notification>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        SimularErrorInterno(usuarioId);

        List<Notification> notificaciones = await _repository.ListarPorUsuarioAsync(usuarioId);
        if (notificaciones.Count == 0)
        {
            throw new NotFoundException(ErrorCodes.NTF_003, "No se encontraron notificaciones para el usuario.");
        }

        return notificaciones;
    }

    // ── Métodos de apoyo ──────────────────────────────────────────────────

    // Prueba del error 500 (NTF-004): se mantiene para la demo, documentado en el README
    private static void SimularErrorInterno(Guid id)
    {
        if (id == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}
