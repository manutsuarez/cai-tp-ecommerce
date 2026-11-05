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

    // ── OBTENER POR EMAIL ─────────────────────────────────────────────────
    public async Task<User?> ObtenerPorEmailAsync(string email)
    {
        using SqliteConnection conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>("""
            SELECT Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos
            FROM usuarios
            WHERE Email = @Email
            """, new { Email = email });
    }

    // ── INSERTAR ──────────────────────────────────────────────────────────
    public async Task InsertarAsync(User usuario)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            INSERT INTO usuarios (Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos)
            VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos)
            """, new
        {
            Id = usuario.Id.ToString(),
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            PasswordHash = usuario.PasswordHash,
            FechaRegistro = usuario.FechaRegistro.ToString("O"),
            Activo = usuario.Activo,
            IntentosFallidos = usuario.IntentosFallidos
        });
    }

    // ── ACTUALIZAR ESTADO DE ACCESO (intentos fallidos y bloqueo) ─────────
    public async Task ActualizarAccesoAsync(Guid id, bool activo, int intentosFallidos)
    {
        using SqliteConnection conn = CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE usuarios
            SET Activo = @Activo,
                IntentosFallidos = @IntentosFallidos
            WHERE Id = @Id
            """, new { Id = id.ToString(), Activo = activo, IntentosFallidos = intentosFallidos });
    }
}