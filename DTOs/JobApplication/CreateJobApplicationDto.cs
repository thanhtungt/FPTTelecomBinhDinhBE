using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.JobApplication;

public class CreateJobApplicationDto
{
    [Required(ErrorMessage = "ID tin tuyển dụng là bắt buộc")]
    public int JobPostingId { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
    public string? Address { get; set; }

    public string? CoverLetter { get; set; }

    [Url(ErrorMessage = "URL CV không hợp lệ")]
    [StringLength(500, ErrorMessage = "URL CV không được quá 500 ký tự")]
    public string? ResumeUrl { get; set; }
}