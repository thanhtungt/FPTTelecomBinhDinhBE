namespace FPTTelecomBE.DTOs.JobApplication;

public class JobApplicationResponseDto
{
    public int Id { get; set; }
    public int JobPostingId { get; set; }
    public string? JobPostingTitle { get; set; }
    public string? JobPostingPosition { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ReviewedByUserId { get; set; }
    public string? ReviewedByUserName { get; set; }
    public string? ReviewNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}