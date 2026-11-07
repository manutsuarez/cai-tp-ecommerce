using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Data;

/// <summary>
/// Acceso a las tablas de órdenes e ítems. Solo hace consultas SQL; la lógica va en el service.
/// </summary>
public class OrderRepository
{
    private readonly IConfiguration _config;

    public OrderRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db");
    }

    public async Task<List<Order>> ListarAsync(Guid? usuarioId)
    {
        string? usuario = null;
        if (usuarioId != null)
        {
            usuario = usuarioId.Value.ToString();
        }

        using SqliteConnection conn = CreateConnection();
        IEnumerable<Order> filas = await conn.QueryAsync<Order>("""
            SELECT Id, UsuarioId, Total, Estado, FechaCreacion
            FROM ordenes
            WHERE (@UsuarioId IS NULL OR UsuarioId = @UsuarioId)
            ORDER BY FechaCreacion
            """, new { UsuarioId = usuario });

        List<Order> ordenes = new List<Order>(filas);
        foreach (Order orden in ordenes)
        {
            orden.Items = await ObtenerItemsAsync(conn, orden.Id);
        }

        return ordenes;
    }

    public async Task<Order?> ObtenerPorIdAsync(Guid id)
    {
        using SqliteConnection conn = CreateConnection();
        Order? orden = await conn.QuerySingleOrDefaultAsync<Order>("""
            SELECT Id, UsuarioId, Total, Estado, FechaCreacion
            FROM ordenes
            WHERE Id = @Id
            """, new { Id = id.ToString() });

        if (orden != null)
        {
            orden.Items = await ObtenerItemsAsync(conn, orden.Id);
        }

        return orden;
    }

    public async Task ActualizarEstadoAsync(Guid id, string estado, DateTime fechaActualizacion)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE ordenes
            SET Estado = @Estado,
                FechaActualizacion = @FechaActualizacion
            WHERE Id = @Id
            """, new
        {
            Id = id.ToString(),
            Estado = estado,
            FechaActualizacion = fechaActualizacion.ToUniversalTime().ToString("O")
        });
    }

    public async Task<List<OrderByProductResponse>> ListarPorProductoAsync(Guid productoId)
    {
        using SqliteConnection conn = CreateConnection();
        IEnumerable<OrderByProductResponse> filas = await conn.QueryAsync<OrderByProductResponse>("""
            SELECT o.Id, o.Estado
            FROM ordenes o
            WHERE EXISTS (
                SELECT 1
                FROM orden_items i
                WHERE i.OrdenId = o.Id
                  AND i.ProductoId = @ProductoId
            )
            ORDER BY o.FechaCreacion
            """, new { ProductoId = productoId.ToString() });

        return new List<OrderByProductResponse>(filas);
    }

    private static async Task<List<OrderItem>> ObtenerItemsAsync(SqliteConnection conn, Guid ordenId)
    {
        IEnumerable<OrderItem> filas = await conn.QueryAsync<OrderItem>("""
            SELECT ProductoId, Cantidad, PrecioUnitario
            FROM orden_items
            WHERE OrdenId = @OrdenId
            ORDER BY Id
            """, new { OrdenId = ordenId.ToString() });

        return new List<OrderItem>(filas);
    }
}