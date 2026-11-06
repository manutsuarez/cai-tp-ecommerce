using Dapper;
using Microsoft.Data.Sqlite;

namespace Products.API.Data;

/// <summary>
/// Crea la tabla de productos al arrancar el servicio (si no existe)
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
        string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db";

        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        // Guid y fechas se guardan como texto; el precio como número
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS productos (
                Id            TEXT    PRIMARY KEY,
                Nombre        TEXT    NOT NULL,
                Descripcion   TEXT,
                Precio        REAL    NOT NULL,
                Stock         INTEGER NOT NULL,
                Categoria     TEXT    NOT NULL,
                FechaCreacion TEXT    NOT NULL
            );
            """);

        long cantidad = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM productos");
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
            INSERT INTO productos (Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion)
            VALUES (@Id, @Nombre, @Descripcion, @Precio, @Stock, @Categoria, @FechaCreacion);
            """;

        // Notebook del enunciado (tiene una orden pendiente en Orders → sirve para mostrar PRD-004)
        connection.Execute(sql, new
        {
            Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            Nombre = "Notebook Dell XPS 15",
            Descripcion = "Laptop 15 pulgadas, 32GB RAM",
            Precio = 1500.00m,
            Stock = 10,
            Categoria = "Electrónica",
            FechaCreacion = "2024-01-15T10:30:00.0000000Z"
        });

        // Segundo producto del enunciado (está en el carrito de María)
        connection.Execute(sql, new
        {
            Id = "aaaabbbb-cccc-dddd-eeee-ffff00001111",
            Nombre = "Silla de Oficina Ergonómica",
            Descripcion = "Silla con soporte lumbar y apoyabrazos ajustables",
            Precio = 120.50m,
            Stock = 8,
            Categoria = "Hogar y Deco",
            FechaCreacion = "2024-01-16T09:00:00.0000000Z"
        });

        // Producto sin órdenes: sirve para mostrar un DELETE exitoso (204)
        connection.Execute(sql, new
        {
            Id = "bbbbcccc-dddd-eeee-ffff-000011112222",
            Nombre = "Remera Deportiva",
            Descripcion = "Remera técnica de secado rápido",
            Precio = 35.90m,
            Stock = 20,
            Categoria = "Deportes",
            FechaCreacion = "2024-01-17T08:30:00.0000000Z"
        });
    }
}
