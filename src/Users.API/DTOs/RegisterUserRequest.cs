using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;

/// <summary>
/// Datos para registrar un nuevo usuario.
/// </summary>
public class RegisterUserRequest
{
    /// <summary>Nombre del usuario. Requerido.</summary>
    /// <example>María</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del usuario. Requerido.</summary>
    /// <example>González</example>
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Email del usuario. Requerido, único y con formato válido.</summary>
    /// <example>maria@email.com</example>
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña. Requerida. Nunca se devuelve en las respuestas.</summary>
    /// <example>MiPassword123!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}