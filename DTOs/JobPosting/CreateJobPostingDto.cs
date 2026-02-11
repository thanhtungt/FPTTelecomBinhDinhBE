using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.JobPosting;

public class CreateJobPostingDto
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả công việc là bắt buộc")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vị trí tuyển dụng là bắt buộc")]
    [StringLength(100, ErrorMessage = "Vị trí không được quá 100 ký tự")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phòng ban là bắt buộc")]
    [StringLength(100, ErrorMessage = "Phòng ban không được quá 100 ký tự")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa điểm làm việc là bắt buộc")]
    [StringLength(200, ErrorMessage = "Địa điểm không được quá 200 ký tự")]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string EmploymentType { get; set; } = "Full-time";

    [StringLength(50)]
    public string ExperienceLevel { get; set; } = "Junior";

    [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối thiểu phải lớn hơn 0")]
    public decimal? SalaryMin { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối đa phải lớn hơn 0")]
    public decimal? SalaryMax { get; set; }

    public string? Requirements { get; set; }

    public string? Benefits { get; set; }

    [Range(1, 1000, ErrorMessage = "Số lượng tuyển dụng phải từ 1 đến 1000")]
    public int NumberOfPositions { get; set; } = 1;

    public DateTime? ApplicationDeadline { get; set; }
}