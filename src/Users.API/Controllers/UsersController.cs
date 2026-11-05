using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Services;

namespace Users.API.Controllers;

/// <summary>
/// Registro y autenticación de usuarios.
/// </summary>
/// <remarks>
/// Datos para probar errores (respuestas de prueba): register con email existe@email.com → USR-001;
/// login con password "incorrecta" → USR-003; login con bloqueado@email.com → USR-004;
/// login con fraude@email.com → USR-005; email mal formado o campos vacíos → USR-002;
/// register con email falla@email.com → USR-006 (falla simulada).
/// </remarks>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult Register(RegisterUserRequest request)
    {
        SimularErrorInterno(request.Email);
        ValidarEmailNoRegistrado(request.Email);

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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult Login(LoginRequest request)
    {
        ValidarUsuarioNoBloqueado(request.Email);
        ValidarCredenciales(request.Password);

        LoginResponse response = new LoginResponse
        {
            Id = new Guid("a1b2c3d4-0000-0000-0000-111122223333"),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com"
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtiene un usuario por su ID. Uso interno entre servicios (Orders y Notifications):
    /// no forma parte del contrato público, por eso no aparece en Swagger.
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <response code="200">Usuario encontrado (sin PasswordHash).</response>
    /// <response code="404">El usuario no existe.</response>
    [HttpGet("{id:guid}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetById(Guid id)
    {
        UserResponse? usuario = await _userService.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    // --- Validaciones de prueba (se reemplazan cuando haya lógica real) ---

    private static void ValidarEmailNoRegistrado(string email)
    {
        if (email == "existe@email.com")
        {
            throw new BusinessRuleException(ErrorCodes.USR_001,
                "El email '" + email + "' ya está registrado.");
        }
    }

    private static void ValidarUsuarioNoBloqueado(string email)
    {
        if (email == "bloqueado@email.com")
        {
            throw new BusinessRuleException(ErrorCodes.USR_004,
                "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.");
        }

        if (email == "fraude@email.com")
        {
            throw new BusinessRuleException(ErrorCodes.USR_005,
                "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.");
        }
    }

    private static void ValidarCredenciales(string password)
    {
        if (password == "incorrecta")
        {
            throw new BusinessRuleException(ErrorCodes.USR_003, "Credenciales incorrectas.");
        }
    }

    private static void SimularErrorInterno(string email)
    {
        if (email == "falla@email.com")
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}