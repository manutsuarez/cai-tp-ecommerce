using Users.API.DTOs;

namespace Users.API.Services;

/// <summary>
/// Lógica de negocio de usuarios. El controller solo llama a estos métodos.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Busca un usuario por su ID. Devuelve null si no existe.
    /// </summary>
    Task<UserResponse?> ObtenerPorIdAsync(Guid id);
}