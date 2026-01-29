using System.ComponentModel.DataAnnotations;

namespace FPTWifiBE.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User"; // "User", "Admin", "Staff"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}