using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Post;

public class CreatePostDto
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung là bắt buộc")]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public string Category { get; set; } = string.Empty; // "khuyen-mai", "tin-tuc", "huong-dan"
}