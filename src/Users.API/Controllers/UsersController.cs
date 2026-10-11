using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // POST /api/users/register
    [HttpPost("register")]
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

    // POST /api/users/login
    [HttpPost("login")]
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