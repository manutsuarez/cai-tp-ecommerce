using Dapper;
using Microsoft.Data.Sqlite;
using Users.API.Models;

namespace Users.API.Data;

/// <summary>
/// Acceso a la tabla de usuarios. Solo hace consultas SQL; la lógica va en el service.
/// </summary>
public class UserRepository
{
    private readonly IConfiguration _config;

    public UserRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=users.db");
    }

    // ── OBTENER POR ID ────────────────────────────────────────────────────
    public async Task<User?> ObtenerPorIdAsync(Guid id)
    {
        using SqliteConnection conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>("""
            SELECT Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos
            FROM usuarios
            WHERE Id = @Id
            """, new { Id = id.ToString() });
    }
}