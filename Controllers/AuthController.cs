using FPTTelecomBE.DTOs.Auth;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.Register(dto);
        if (result == null)
            return BadRequest(new { message = "Số điện thoại đã được đăng ký" });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (result == null)
            return Unauthorized(new { message = "Số điện thoại hoặc mật khẩu không đúng" });

        return Ok(result);
    }

    // Test endpoint to verify JWT authentication
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userPhone = User.FindFirst(ClaimTypes.MobilePhone)?.Value;

        return Ok(new
        {
            userId,
            userName,
            userRole,
            userPhone,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
}