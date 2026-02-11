using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.JobPosting;

public class UpdateJobPostingDto
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả công việc là bắt buộc")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vị trí tuyển dụng là bắt buộc")]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phòng ban là bắt buộc")]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa điểm làm việc là bắt buộc")]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string EmploymentType { get; set; } = "Full-time";

    [StringLength(50)]
    public string ExperienceLevel { get; set; } = "Junior";

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? Requirements { get; set; }

    public string? Benefits { get; set; }

    [Range(1, 1000)]
    public int NumberOfPositions { get; set; } = 1;

    public DateTime? ApplicationDeadline { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20)]
    public string Status { get; set; } = "draft";
}