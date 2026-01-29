using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}