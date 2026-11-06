using Dapper;
using Microsoft.Data.Sqlite;

namespace Orders.API.Data;

/// <summary>
/// Crea las tablas de órdenes e ítems al arrancar el servicio (si no existen)
/// y carga los datos de prueba si la tabla de órdenes está vacía.
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
        string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";

        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS ordenes (
                Id                 TEXT PRIMARY KEY,
                UsuarioId          TEXT NOT NULL,
                Total              REAL NOT NULL,
                Estado             TEXT NOT NULL,
                FechaCreacion      TEXT NOT NULL,
                FechaActualizacion TEXT
            );
            """);

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS orden_items (
                Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                OrdenId        TEXT    NOT NULL,
                ProductoId     TEXT    NOT NULL,
                Cantidad       INTEGER NOT NULL,
                PrecioUnitario REAL    NOT NULL,
                FOREIGN KEY (OrdenId) REFERENCES ordenes(Id)
            );
            """);

        long cantidad = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM ordenes");
        if (cantidad == 0)
        {
            CargarDatosDePrueba(connection);
        }

        _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
    }

    private static void CargarDatosDePrueba(SqliteConnection connection)
    {
        connection.Execute("""
            INSERT INTO ordenes (Id, UsuarioId, Total, Estado, FechaCreacion)
            VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion);
            """, new
        {
            Id = "f1e2d3c4-0000-0000-0000-aabbccddeeff",
            UsuarioId = "a1b2c3d4-0000-0000-0000-111122223333",
            Total = 3000.00m,
            Estado = "Pendiente",
            FechaCreacion = "2024-03-10T11:00:00.0000000Z"
        });

        connection.Execute("""
            INSERT INTO orden_items (OrdenId, ProductoId, Cantidad, PrecioUnitario)
            VALUES (@OrdenId, @ProductoId, @Cantidad, @PrecioUnitario);
            """, new
        {
            OrdenId = "f1e2d3c4-0000-0000-0000-aabbccddeeff",
            ProductoId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            Cantidad = 2,
            PrecioUnitario = 1500.00m
        });
    }
}