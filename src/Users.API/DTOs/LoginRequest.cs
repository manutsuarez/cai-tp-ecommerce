namespace Users.API.DTOs;

/// <summary>
/// Credenciales para iniciar sesión.
/// </summary>
public class LoginRequest
{
    /// <summary>Email del usuario.</summary>
    /// <example>maria@email.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña del usuario.</summary>
    /// <example>MiPassword123!</example>
    public string Password { get; set; } = string.Empty;
}