using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Data;

/// <summary>
/// Crea las tablas del carrito al arrancar el servicio (si no existen)
/// y carga los datos de prueba si están vacías.
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
        string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";

        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        // Un carrito por usuario: el ID del carrito es el ID del usuario
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS carritos (
                UsuarioId          TEXT PRIMARY KEY,
                FechaActualizacion TEXT NOT NULL
            );
            """);

        // Los productos de cada carrito: un producto aparece una sola vez por carrito
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS carrito_items (
                UsuarioId  TEXT    NOT NULL,
                ProductoId TEXT    NOT NULL,
                Cantidad   INTEGER NOT NULL,
                PRIMARY KEY (UsuarioId, ProductoId)
            );
            """);

        long cantidad = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM carritos");
        if (cantidad == 0)
        {
            CargarDatosDePrueba(connection);
        }

        _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
    }

    // Datos de prueba: el carrito de María con los IDs del enunciado
    private static void CargarDatosDePrueba(SqliteConnection connection)
    {
        string usuarioMaria = "a1b2c3d4-0000-0000-0000-111122223333";

        connection.Execute("""
            INSERT INTO carritos (UsuarioId, FechaActualizacion)
            VALUES (@UsuarioId, @FechaActualizacion);
            """, new { UsuarioId = usuarioMaria, FechaActualizacion = "2024-03-10T10:45:00.0000000Z" });

        string sqlItem = """
            INSERT INTO carrito_items (UsuarioId, ProductoId, Cantidad)
            VALUES (@UsuarioId, @ProductoId, @Cantidad);
            """;

        // Notebook Dell XPS 15 × 1
        connection.Execute(sqlItem, new
        {
            UsuarioId = usuarioMaria,
            ProductoId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            Cantidad = 1
        });

        // Segundo producto × 3
        connection.Execute(sqlItem, new
        {
            UsuarioId = usuarioMaria,
            ProductoId = "aaaabbbb-cccc-dddd-eeee-ffff00001111",
            Cantidad = 3
        });
    }
}