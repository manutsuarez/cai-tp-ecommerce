using Users.API.DTOs;

namespace Users.API.Services;

/// <summary>
/// Lógica de negocio de usuarios. El controller solo llama a estos métodos.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registra un usuario nuevo. Lanza USR-002 si los datos son inválidos
    /// y USR-001 si el email ya está registrado.
    /// </summary>
    Task<UserResponse> RegistrarAsync(RegisterUserRequest request);

    /// <summary>
    /// Autentica con email y contraseña. Lanza USR-003, USR-004 o USR-005.
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Busca un usuario por su ID. Devuelve null si no existe.
    /// </summary>
    Task<UserResponse?> ObtenerPorIdAsync(Guid id);
}