using HotelApp.Api.DTOs;
using HotelApp.Api.Helpers;
using HotelApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        return Ok(ApiResponse<object>.Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.Role
        }, "User registered successfully."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password."));
        }

        return Ok(ApiResponse<object>.Ok(new { Token = token }, "Login successful."));
    }
}
