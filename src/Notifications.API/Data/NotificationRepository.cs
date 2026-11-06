using Dapper;
using Microsoft.Data.Sqlite;
using Notifications.API.Models;

namespace Notifications.API.Data;

/// <summary>
/// Acceso a la tabla de notificaciones. Solo hace consultas SQL; la lógica va en el service.
/// </summary>
public class NotificationRepository
{
    private readonly IConfiguration _config;

    public NotificationRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=notifications.db");
    }

    // ── LISTAR LAS NOTIFICACIONES DE UN USUARIO (las más nuevas primero) ──
    public async Task<List<Notification>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        using SqliteConnection conn = CreateConnection();
        IEnumerable<Notification> filas = await conn.QueryAsync<Notification>("""
            SELECT Id, UsuarioId, Mensaje, Tipo, Estado, FechaEnvio
            FROM notificaciones
            WHERE UsuarioId = @UsuarioId
            ORDER BY FechaEnvio DESC
            """, new { UsuarioId = usuarioId.ToString() });

        return new List<Notification>(filas);
    }

    // ── INSERTAR ──────────────────────────────────────────────────────────
    public async Task InsertarAsync(Notification notificacion)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO notificaciones (Id, UsuarioId, Mensaje, Tipo, Estado, FechaEnvio)
            VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio)
            """, new
        {
            Id = notificacion.Id.ToString(),
            UsuarioId = notificacion.UsuarioId.ToString(),
            Mensaje = notificacion.Mensaje,
            Tipo = notificacion.Tipo,
            Estado = notificacion.Estado,
            FechaEnvio = notificacion.FechaEnvio.ToUniversalTime().ToString("O")
        });
    }
}
