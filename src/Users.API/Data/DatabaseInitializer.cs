using Dapper;
using Microsoft.Data.Sqlite;

namespace Users.API.Data;

/// <summary>
/// Crea la tabla de usuarios al arrancar el servicio (si no existe)
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
        string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=users.db";

        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        // Guid y fechas se guardan como texto; Activo como 0/1
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS usuarios (
                Id               TEXT    PRIMARY KEY,
                Nombre           TEXT    NOT NULL,
                Apellido         TEXT    NOT NULL,
                Email            TEXT    NOT NULL UNIQUE,
                PasswordHash     TEXT    NOT NULL,
                FechaRegistro    TEXT    NOT NULL,
                Activo           INTEGER NOT NULL,
                IntentosFallidos INTEGER NOT NULL
            );
            """);

        long cantidad = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM usuarios");
        if (cantidad == 0)
        {
            CargarDatosDePrueba(connection);
        }

        _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
    }

    // Datos de prueba: los mismos IDs del enunciado, para que la demo cierre entre servicios
    private static void CargarDatosDePrueba(SqliteConnection connection)
    {
        string sql = """
            INSERT INTO usuarios (Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos)
            VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos);
            """;

        // La contraseña nunca se guarda en texto: se guarda su hash
        string hash = BCrypt.Net.BCrypt.HashPassword("MiPassword123!");

        // María: usuaria activa del enunciado
        connection.Execute(sql, new
        {
            Id = "a1b2c3d4-0000-0000-0000-111122223333",
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            PasswordHash = hash,
            FechaRegistro = "2024-03-10T09:00:00.0000000Z",
            Activo = true,
            IntentosFallidos = 0
        });

        // Bloqueado por 3 intentos fallidos → USR-004
        connection.Execute(sql, new
        {
            Id = "b2c3d4e5-0000-0000-0000-222233334444",
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "bloqueado@email.com",
            PasswordHash = hash,
            FechaRegistro = "2024-03-11T10:00:00.0000000Z",
            Activo = false,
            IntentosFallidos = 3
        });

        // Bloqueado manualmente (fraude) → USR-005
        connection.Execute(sql, new
        {
            Id = "c3d4e5f6-0000-0000-0000-333344445555",
            Nombre = "Laura",
            Apellido = "Fernández",
            Email = "fraude@email.com",
            PasswordHash = hash,
            FechaRegistro = "2024-03-12T11:00:00.0000000Z",
            Activo = false,
            IntentosFallidos = 0
        });
    }
}
