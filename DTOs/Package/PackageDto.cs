namespace FPTTelecomBE.DTOs.Package;

public class PackageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SpeedDown { get; set; }
    public int SpeedUp { get; set; }
    public decimal PriceMonthly { get; set; }
    public string PromotionText { get; set; } = string.Empty;
    public string DeviceBonus { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public bool Active { get; set; }
}