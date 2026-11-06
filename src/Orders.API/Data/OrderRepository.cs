using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.DTOs;

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
}