using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.JobPosting;

public class UpdateJobPostingStatusDto
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;
}