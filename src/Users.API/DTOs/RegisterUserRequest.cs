namespace Users.API.DTOs;

/// <summary>
/// Datos para registrar un nuevo usuario.
/// </summary>
public class RegisterUserRequest
{
    /// <summary>Nombre del usuario.</summary>
    /// <example>María</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del usuario.</summary>
    /// <example>González</example>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Email del usuario. Debe ser único y con formato válido.</summary>
    /// <example>maria@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña. Nunca se devuelve en las respuestas.</summary>
    /// <example>MiPassword123!</example>
    public string Password { get; set; } = string.Empty;
}