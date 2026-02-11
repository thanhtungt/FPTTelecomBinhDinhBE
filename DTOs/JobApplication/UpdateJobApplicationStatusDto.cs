using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.JobApplication;

public class UpdateJobApplicationStatusDto
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    public string? ReviewNote { get; set; }
}