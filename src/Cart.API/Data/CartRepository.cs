using Dapper;
using Microsoft.Data.Sqlite;
using Cart.API.Models;

namespace Cart.API.Data;

/// <summary>
/// Acceso a las tablas del carrito. Solo hace consultas SQL; la lógica va en el service.
/// </summary>
public class CartRepository
{
    private readonly IConfiguration _config;

    public CartRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db");
    }

    // ── OBTENER EL CARRITO DE UN USUARIO (con sus productos) ──────────────
    public async Task<ShoppingCart?> ObtenerPorUsuarioAsync(Guid usuarioId)
    {
        using SqliteConnection conn = CreateConnection();

        ShoppingCart? carrito = await conn.QuerySingleOrDefaultAsync<ShoppingCart>("""
            SELECT UsuarioId, FechaActualizacion
            FROM carritos
            WHERE UsuarioId = @UsuarioId
            """, new { UsuarioId = usuarioId.ToString() });

        if (carrito == null)
        {
            return null;
        }

        // Los productos van en orden de llegada (rowid = orden en que se insertaron)
        IEnumerable<CartItem> filas = await conn.QueryAsync<CartItem>("""
            SELECT ProductoId, Cantidad
            FROM carrito_items
            WHERE UsuarioId = @UsuarioId
            ORDER BY rowid
            """, new { UsuarioId = usuarioId.ToString() });

        carrito.Items = new List<CartItem>(filas);
        return carrito;
    }

    // ── GUARDAR UN PRODUCTO (lo agrega o le cambia la cantidad) ───────────
    // Si el carrito no existe, lo crea (así el primer POST crea el carrito).
    public async Task GuardarItemAsync(Guid usuarioId, Guid productoId, int cantidad, DateTime fecha)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO carritos (UsuarioId, FechaActualizacion)
            VALUES (@UsuarioId, @Fecha)
            ON CONFLICT(UsuarioId) DO UPDATE SET FechaActualizacion = @Fecha;

            INSERT INTO carrito_items (UsuarioId, ProductoId, Cantidad)
            VALUES (@UsuarioId, @ProductoId, @Cantidad)
            ON CONFLICT(UsuarioId, ProductoId) DO UPDATE SET Cantidad = @Cantidad;
            """, new
        {
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString(),
            Cantidad = cantidad,
            Fecha = fecha.ToUniversalTime().ToString("O")
        });
    }

    // ── QUITAR UN PRODUCTO ────────────────────────────────────────────────
    public async Task EliminarItemAsync(Guid usuarioId, Guid productoId, DateTime fecha)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            DELETE FROM carrito_items
            WHERE UsuarioId = @UsuarioId AND ProductoId = @ProductoId;

            UPDATE carritos
            SET FechaActualizacion = @Fecha
            WHERE UsuarioId = @UsuarioId;
            """, new
        {
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString(),
            Fecha = fecha.ToUniversalTime().ToString("O")
        });
    }

    // ── VACIAR EL CARRITO (el carrito sigue existiendo, sin productos) ────
    public async Task VaciarAsync(Guid usuarioId, DateTime fecha)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            DELETE FROM carrito_items
            WHERE UsuarioId = @UsuarioId;

            UPDATE carritos
            SET FechaActualizacion = @Fecha
            WHERE UsuarioId = @UsuarioId;
            """, new
        {
            UsuarioId = usuarioId.ToString(),
            Fecha = fecha.ToUniversalTime().ToString("O")
        });
    }
}