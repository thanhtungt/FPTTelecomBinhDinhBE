using FPTWifiBE.Models;
using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.Models;

public class JobApplication
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    public string? CoverLetter { get; set; }

    [StringLength(500)]
    public string? ResumeUrl { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "pending";

    public int? ReviewedByUserId { get; set; }
    public User? ReviewedBy { get; set; }

    public string? ReviewNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}