namespace FPTTelecomBE.DTOs.Chat;

public class ChatMessageDto
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class SendMessageDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? SenderName { get; set; } // Cho anonymous user
    public string? SenderEmail { get; set; } // Cho anonymous user
}

public class StartChatDto
{
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhone { get; set; }
    public string InitialMessage { get; set; } = string.Empty;
}

public class ChatSessionDto
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhone { get; set; }
    public string? AssignedStaffName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public ChatMessageDto? LastMessage { get; set; }
}