namespace FPTTelecomBE.DTOs.JobPosting;

public class JobPostingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? SalaryCurrency { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public int NumberOfPositions { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ApplicationCount { get; set; }
}