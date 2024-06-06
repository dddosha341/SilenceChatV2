﻿using System;
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

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == request.UserName);
        if (user == null ||
            !_authService.VerifyPasswordHash(
                request.Password, user.PasswordHash, user.Salt))
        {
            return Unauthorized("Invalid username or password.");
        }

        var accessToken = _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _configurationService.JwtRefreshTokenExpirationDays);
        _context.SaveChanges();

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Username = user.UserName
        });
    }

    [HttpPost("refresh-token")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity.Name; 
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid token.");
        }

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        _context.SaveChanges();

        return Ok(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.UserName == request.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }
        _authService.CreatePasswordHash(request.Password,
       out string passwordHash, out string passwordSalt);

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
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
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

