using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Registration;

public class CreateRegistrationDto
{
    public int? UserId { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gói cước là bắt buộc")]
    public int PackageId { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú không được quá 1000 ký tự")]
    public string? Note { get; set; }
}