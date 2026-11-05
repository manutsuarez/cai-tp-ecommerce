using Users.API.Data;
using Users.API.DTOs;
using Users.API.Models;

namespace Users.API.Services;

/// <summary>
/// Lógica de negocio de usuarios: usa el repository para leer y guardar,
/// y arma las respuestas sin exponer nunca el PasswordHash.
/// </summary>
public class UserService : IUserService
{
    private readonly UserRepository _repository;

    public UserService(UserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponse?> ObtenerPorIdAsync(Guid id)
    {
        User? usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            return null;
        }

        return ConvertirAResponse(usuario);
    }

    // Pasa del modelo (con PasswordHash) al DTO de respuesta (sin PasswordHash)
    private static UserResponse ConvertirAResponse(User usuario)
    {
        return new UserResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            FechaRegistro = usuario.FechaRegistro,
            Activo = usuario.Activo
        };
    }
}