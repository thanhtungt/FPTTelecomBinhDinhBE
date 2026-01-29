using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Registration;

public class UpdateRegistrationStatusDto
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public string Status { get; set; } = string.Empty;

    public int? AssignedStaffId { get; set; }
}