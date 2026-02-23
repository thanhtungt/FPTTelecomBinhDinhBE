using FPTWifiBE.Models;

namespace FPTTelecomBE.Models;

public class ChatSession
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int? UserId { get; set; } // Null nếu anonymous user
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhone { get; set; }
    public int? AssignedStaffId { get; set; } // Staff được gán phụ trách
    public string Status { get; set; } = "waiting"; // "waiting", "active", "resolved", "closed"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Navigation
    public User? User { get; set; }
    public User? AssignedStaff { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}