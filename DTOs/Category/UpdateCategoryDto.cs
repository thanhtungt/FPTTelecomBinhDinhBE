using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Category;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
    public string? Description { get; set; }

    [Range(0, 1000, ErrorMessage = "Thứ tự hiển thị phải từ 0 đến 1000")]
    public int DisplayOrder { get; set; } = 0;

    public bool Active { get; set; } = true;
}