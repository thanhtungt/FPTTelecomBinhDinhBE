using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "Thứ tự hiển thị phải từ 0 đến 1000")]
    public int DisplayOrder { get; set; } = 0;

    public bool Active { get; set; } = true;
}