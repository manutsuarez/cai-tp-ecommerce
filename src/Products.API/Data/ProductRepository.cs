using Dapper;
using Microsoft.Data.Sqlite;
using Products.API.Models;

namespace Products.API.Data;

/// <summary>
/// Acceso a la tabla de productos. Solo hace consultas SQL; la lógica va en el service.
/// </summary>
public class ProductRepository
{
    private readonly IConfiguration _config;

    public ProductRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db");
    }

    // ── LISTAR (con filtros opcionales por categoría y por nombre) ────────
    public async Task<List<Product>> ListarAsync(string? categoria, string? nombre)
    {
        using SqliteConnection conn = CreateConnection();
        IEnumerable<Product> filas = await conn.QueryAsync<Product>("""
            SELECT Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion
            FROM productos
            WHERE (@Categoria IS NULL OR Categoria = @Categoria)
              AND (@Nombre IS NULL OR Nombre LIKE '%' || @Nombre || '%')
            ORDER BY Nombre
            """, new { Categoria = categoria, Nombre = nombre });

        return new List<Product>(filas);
    }

    // ── OBTENER POR ID ────────────────────────────────────────────────────
    public async Task<Product?> ObtenerPorIdAsync(Guid id)
    {
        using SqliteConnection conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Product>("""
            SELECT Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion
            FROM productos
            WHERE Id = @Id
            """, new { Id = id.ToString() });
    }

    // ── ¿YA EXISTE UN PRODUCTO CON ESE NOMBRE EN ESA CATEGORÍA? (PRD-003) ─
    public async Task<bool> ExistePorNombreYCategoriaAsync(string nombre, string categoria)
    {
        using SqliteConnection conn = CreateConnection();
        long cantidad = await conn.ExecuteScalarAsync<long>("""
            SELECT COUNT(*)
            FROM productos
            WHERE Nombre = @Nombre COLLATE NOCASE
              AND Categoria = @Categoria COLLATE NOCASE
            """, new { Nombre = nombre, Categoria = categoria });

        return cantidad > 0;
    }

    // ── INSERTAR ──────────────────────────────────────────────────────────
    public async Task InsertarAsync(Product producto)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO productos (Id, Nombre, Descripcion, Precio, Stock, Categoria, FechaCreacion)
            VALUES (@Id, @Nombre, @Descripcion, @Precio, @Stock, @Categoria, @FechaCreacion)
            """, new
        {
            Id = producto.Id.ToString(),
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            Categoria = producto.Categoria,
            FechaCreacion = producto.FechaCreacion.ToUniversalTime().ToString("O")
        });
    }

    // ── ACTUALIZAR (la fecha de creación no cambia) ───────────────────────
    public async Task ActualizarAsync(Product producto)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE productos
            SET Nombre = @Nombre,
                Descripcion = @Descripcion,
                Precio = @Precio,
                Stock = @Stock,
                Categoria = @Categoria
            WHERE Id = @Id
            """, new
        {
            Id = producto.Id.ToString(),
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            Categoria = producto.Categoria
        });
    }

    // ── ELIMINAR ──────────────────────────────────────────────────────────
    public async Task EliminarAsync(Guid id)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("DELETE FROM productos WHERE Id = @Id", new { Id = id.ToString() });
    }
}
