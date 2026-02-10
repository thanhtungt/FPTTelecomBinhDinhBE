using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Package;

public class CreatePackageDto
{
    [Required(ErrorMessage = "Tên gói cước là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên gói cước không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tốc độ download là bắt buộc")]
    [Range(1, 100000, ErrorMessage = "Tốc độ download phải từ 1 đến 100000 Mbps")]
    public int SpeedDown { get; set; }

    [Required(ErrorMessage = "Tốc độ upload là bắt buộc")]
    [Range(1, 100000, ErrorMessage = "Tốc độ upload phải từ 1 đến 100000 Mbps")]
    public int SpeedUp { get; set; }

    [Required(ErrorMessage = "Giá hàng tháng là bắt buộc")]
    [Range(0, 10000000, ErrorMessage = "Giá phải từ 0 đến 10,000,000 VND")]
    public decimal PriceMonthly { get; set; }

    [StringLength(500, ErrorMessage = "Text khuyến mãi không được quá 500 ký tự")]
    public string PromotionText { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Thiết bị tặng kèm không được quá 200 ký tự")]
    public string DeviceBonus { get; set; } = string.Empty;

    [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public int CategoryId { get; set; }

    public bool Active { get; set; } = true;
}