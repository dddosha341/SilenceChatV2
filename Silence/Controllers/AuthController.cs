using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silence.Web.Entities;
using Silence.Web.Services;
using Silence.Web.Data;
using System.Linq;
namespace Silence.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ConfigurationService _configurationService;
    private readonly AppDbContext _context;

    public AuthController(
        AuthService authService,
        ConfigurationService configurationService,
        AppDbContext context
        )
    {
        _authService = authService;
        _configurationService = configurationService;
        _context = context;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Проверяем учетные данные пользователя
        var user = _context.Users.SingleOrDefault(u => u.UserName == request.UserName);
        if (user == null ||
            !_authService.VerifyPasswordHash(
                request.Password, user.PasswordHash, user.Salt))
        {
            return Unauthorized("Invalid username or password.");
        }

        // Генерируем access и refresh токены
        var accessToken = _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        // Сохраняем refresh токен в базе данных
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _configurationService.JwtRefreshTokenExpirationDays);
        _context.SaveChanges();

        // Возвращаем токены клиенту
        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Username = user.UserName
        });
    }

    // POST api/auth/refresh
    [HttpPost("refresh-token")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity.Name; // Извлекаем имя пользователя из принципала
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);

        // Проверяем валидность refresh токена
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid token.");
        }

        // Генерируем новые токены
        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        // Обновляем refresh токен в базе данных
        user.RefreshToken = newRefreshToken;
        // TODO: save new life time
        _context.SaveChanges();

        // Возвращаем новые токены клиенту
        return Ok(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Проверяем, существует ли уже пользователь с таким именем
        var existingUser = _context.Users.FirstOrDefault(u => u.UserName == request.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }
        _authService.CreatePasswordHash(request.Password,
       out string passwordHash, out string passwordSalt);


        // Создаем нового пользователя
        var user = new User
        {
            UserName = request.Username,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Salt = passwordSalt
        };

        try
        {
            // Сохраняем пользователя в базе данных
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Обработка ошибки сохранения пользователя
            return BadRequest("Error registering user.");
        }

        return Ok("User registered successfully.");
    }

    [Authorize]
    [HttpGet("validate-token")]
    public IActionResult ValidateToken()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(new ValidateTokenResponse()
            {
                UserId = user.Id,
                Username = user.UserName
            });
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user is null)
            {
                return Unauthorized();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _context.SaveChanges();

            return Ok();
        }

        return Unauthorized();
    }
}

