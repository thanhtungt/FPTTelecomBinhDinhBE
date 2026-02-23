using FPTWifiBE.Models;

namespace FPTTelecomBE.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty; // Unique session cho mỗi cuộc chat
    public int? UserId { get; set; } // Null nếu user chưa login
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderType { get; set; } = "user"; // "user", "admin", "staff"
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // Navigation
    public User? User { get; set; }
}