using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;

namespace Users.API.Controllers;

/// <summary>
/// Registro y autenticación de usuarios.
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <param name="request">Datos del usuario a registrar.</param>
    /// <response code="201">Usuario registrado correctamente.</response>
    /// <response code="400">USR-002: Los datos del usuario son inválidos.</response>
    /// <response code="409">USR-001: El email ya está registrado.</response>
    /// <response code="500">USR-006: Error interno al procesar el usuario.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Register(RegisterUserRequest request)
    {
        UserResponse response = new UserResponse
        {
            Id = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            FechaRegistro = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc),
            Activo = true
        };

        return StatusCode(201, response);
    }

    /// <summary>
    /// Autentica a un usuario con email y contraseña.
    /// </summary>
    /// <remarks>
    /// Al acumular 3 intentos fallidos consecutivos, el usuario queda bloqueado.
    /// </remarks>
    /// <param name="request">Email y contraseña.</param>
    /// <response code="200">Login correcto.</response>
    /// <response code="400">USR-002: Los datos del usuario son inválidos.</response>
    /// <response code="401">USR-003: Credenciales incorrectas.</response>
    /// <response code="403">USR-004: Usuario bloqueado por demasiados intentos fallidos. / USR-005: Usuario bloqueado por detección de fraude.</response>
    /// <response code="500">USR-006: Error interno al procesar el usuario.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Login(LoginRequest request)
    {
        LoginResponse response = new LoginResponse
        {
            Id = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com"
        };

        return Ok(response);
    }
}