using Dapper;
using Microsoft.Data.Sqlite;

namespace Notifications.API.Data;

/// <summary>
/// Crea la tabla de notificaciones al arrancar el servicio (si no existe)
/// y carga los datos de prueba si la tabla está vacía.
/// </summary>
public class DatabaseInitializer
{
    private readonly IConfiguration _config;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration config, ILogger<DatabaseInitializer> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void Initialize()
    {
        string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=notifications.db";

        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        // Guid y fechas se guardan como texto
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS notificaciones (
                Id         TEXT PRIMARY KEY,
                UsuarioId  TEXT NOT NULL,
                Mensaje    TEXT NOT NULL,
                Tipo       TEXT NOT NULL,
                Estado     TEXT NOT NULL,
                FechaEnvio TEXT NOT NULL
            );
            """);

        long cantidad = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM notificaciones");
        if (cantidad == 0)
        {
            CargarDatosDePrueba(connection);
        }

        _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
    }

    // Dato de prueba: la notificación del enunciado, para María (el mismo usuario que en Users)
    private static void CargarDatosDePrueba(SqliteConnection connection)
    {
        connection.Execute("""
            INSERT INTO notificaciones (Id, UsuarioId, Mensaje, Tipo, Estado, FechaEnvio)
            VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio);
            """, new
        {
            Id = "11112222-3333-4444-5555-666677778888",
            UsuarioId = "a1b2c3d4-0000-0000-0000-111122223333",
            Mensaje = "Su orden #f1e2d3c4 fue confirmada.",
            Tipo = "Email",
            Estado = "Enviada",
            FechaEnvio = "2024-03-10T12:01:00.0000000Z"
        });
    }
}
