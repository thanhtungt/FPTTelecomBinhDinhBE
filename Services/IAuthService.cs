using FPTTelecomBE.DTOs.Auth;

namespace FPTTelecomBE.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> Register(RegisterDto dto);
    Task<AuthResponseDto?> Login(LoginDto dto);
}