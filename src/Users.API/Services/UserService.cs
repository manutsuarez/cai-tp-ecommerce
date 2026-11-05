using System.ComponentModel.DataAnnotations;
using Users.API.Data;
using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Models;
using ValidationException = Users.API.Exceptions.ValidationException;

namespace Users.API.Services;

/// <summary>
/// Lógica de negocio de usuarios: registro, login con bloqueo por intentos
/// y consulta por ID. Nunca expone el PasswordHash.
/// </summary>
public class UserService : IUserService
{
    // Con 3 intentos fallidos seguidos el usuario queda bloqueado (consigna)
    private const int MaximoIntentosFallidos = 3;

    private readonly UserRepository _repository;

    public UserService(UserRepository repository)
    {
        _repository = repository;
    }

    // ── REGISTER ──────────────────────────────────────────────────────────
    public async Task<UserResponse> RegistrarAsync(RegisterUserRequest request)
    {
        SimularErrorInterno(request.Email);
        ValidarDatosDeRegistro(request);

        // El email se guarda en minúsculas para que "Maria@" y "maria@" sean el mismo
        string email = request.Email.Trim().ToLower();

        User? existente = await _repository.ObtenerPorEmailAsync(email);
        if (existente != null)
        {
            throw new BusinessRuleException(ErrorCodes.USR_001,
                "El email '" + email + "' ya está registrado.");
        }

        User usuario = new User
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FechaRegistro = DateTime.UtcNow,
            Activo = true,
            IntentosFallidos = 0
        };

        await _repository.InsertarAsync(usuario);

        return ConvertirAResponse(usuario);
    }

    // ── LOGIN ─────────────────────────────────────────────────────────────
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        string email = request.Email.Trim().ToLower();

        // Si el email no existe se responde lo mismo que con contraseña incorrecta,
        // así no se revela qué emails están registrados
        User? usuario = await _repository.ObtenerPorEmailAsync(email);
        if (usuario == null)
        {
            throw new BusinessRuleException(ErrorCodes.USR_003, "Credenciales incorrectas.");
        }

        // Usuario bloqueado: por intentos (USR-004) o manualmente (USR-005)
        if (!usuario.Activo)
        {
            if (usuario.IntentosFallidos >= MaximoIntentosFallidos)
            {
                throw new BusinessRuleException(ErrorCodes.USR_004,
                    "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.");
            }

            throw new BusinessRuleException(ErrorCodes.USR_005,
                "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.");
        }

        // Contraseña incorrecta: suma un intento; al 3.º se bloquea (pero esta respuesta sigue siendo USR-003)
        bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
        if (!passwordCorrecta)
        {
            int intentos = usuario.IntentosFallidos + 1;
            bool activo = true;
            if (intentos >= MaximoIntentosFallidos)
            {
                activo = false;
            }

            await _repository.ActualizarAccesoAsync(usuario.Id, activo, intentos);
            throw new BusinessRuleException(ErrorCodes.USR_003, "Credenciales incorrectas.");
        }

        // Login correcto: se resetean los intentos fallidos
        if (usuario.IntentosFallidos > 0)
        {
            await _repository.ActualizarAccesoAsync(usuario.Id, true, 0);
        }

        return new LoginResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email
        };
    }

    // ── OBTENER POR ID (uso interno) ──────────────────────────────────────
    public async Task<UserResponse?> ObtenerPorIdAsync(Guid id)
    {
        User? usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            return null;
        }

        return ConvertirAResponse(usuario);
    }

    // ── Métodos de apoyo ──────────────────────────────────────────────────

    // USR-002: junta todos los problemas y los devuelve separados por "; "
    private static void ValidarDatosDeRegistro(RegisterUserRequest request)
    {
        List<string> problemas = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            problemas.Add("El nombre es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            problemas.Add("El apellido es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            problemas.Add("El email es obligatorio.");
        }
        else
        {
            EmailAddressAttribute validadorEmail = new EmailAddressAttribute();
            if (!validadorEmail.IsValid(request.Email.Trim()))
            {
                problemas.Add("El email no tiene un formato válido.");
            }
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            problemas.Add("La contraseña es obligatoria.");
        }

        if (problemas.Count > 0)
        {
            throw new ValidationException(ErrorCodes.USR_002, string.Join("; ", problemas));
        }
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

    // Prueba del error 500 (USR-006): se mantiene para la demo, documentado en el README
    private static void SimularErrorInterno(string email)
    {
        if (email == "falla@email.com")
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}