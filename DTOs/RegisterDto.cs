using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
    public string Password { get; set; } = string.Empty;
}