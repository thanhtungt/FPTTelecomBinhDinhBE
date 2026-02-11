using FPTWifiBE.Models;
using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.Models;

public class JobPosting
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string EmploymentType { get; set; } = "Full-time";

    [StringLength(50)]
    public string ExperienceLevel { get; set; } = "Junior";

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    [StringLength(50)]
    public string? SalaryCurrency { get; set; } = "VND";

    public string? Requirements { get; set; }

    public string? Benefits { get; set; }

    public int NumberOfPositions { get; set; } = 1;

    public DateTime? ApplicationDeadline { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "draft";

    public int CreatedByUserId { get; set; }
    public User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobApplication>? Applications { get; set; }
}