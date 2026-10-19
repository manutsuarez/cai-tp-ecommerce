using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;

/// <summary>
/// Credenciales para iniciar sesión.
/// </summary>
public class LoginRequest
{
    /// <summary>Email del usuario. Requerido.</summary>
    /// <example>maria@email.com</example>
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña del usuario. Requerida.</summary>
    /// <example>MiPassword123!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}