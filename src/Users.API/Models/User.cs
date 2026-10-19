namespace Users.API.Models;

/// <summary>
/// Usuario registrado en el sistema.
/// </summary>
public class User
{
    /// <summary>Identificador único del usuario.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del usuario. Requerido.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del usuario. Requerido.</summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Email del usuario. Requerido, único y con formato válido.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash de la contraseña. Nunca se expone en las respuestas.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Fecha de registro (UTC), asignada automáticamente al registrar.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Falso cuando el usuario está bloqueado.</summary>
    public bool Activo { get; set; }

    /// <summary>Intentos de login fallidos consecutivos. Se resetea al loguearse bien; con 3, el usuario se bloquea.</summary>
    public int IntentosFallidos { get; set; }
}