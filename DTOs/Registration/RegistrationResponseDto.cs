namespace FPTTelecomBE.DTOs.Registration;

public class RegistrationResponseDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal PackagePrice { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}