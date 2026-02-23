using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Chat;
using FPTTelecomBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ChatService> _logger;
    private readonly IChatbotService _chatbotService;

    public ChatService(AppDbContext context, ILogger<ChatService> logger, IChatbotService chatbotService)
    {
        _context = context;
        _logger = logger;
        _chatbotService = chatbotService;
    }

    public async Task<ChatSessionDto> StartChatSession(StartChatDto dto, int? userId)
    {
        var sessionId = Guid.NewGuid().ToString();

        var session = new ChatSession
        {
            SessionId = sessionId,
            UserId = userId,
            UserName = dto.UserName,
            UserEmail = dto.UserEmail,
            UserPhone = dto.UserPhone,
            Status = "bot", // Bắt đầu với bot, chưa phải "waiting"
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        _context.ChatSessions.Add(session);

        // Tin nhắn đầu tiên từ user
        var userMessage = new ChatMessage
        {
            SessionId = sessionId,
            UserId = userId,
            SenderName = dto.UserName ?? "Anonymous",
            SenderEmail = dto.UserEmail ?? "",
            SenderType = "user",
            Message = dto.InitialMessage,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(userMessage);

        // Bot tự động trả lời welcome message
        var welcomeMessage = new ChatMessage
        {
            SessionId = sessionId,
            UserId = null,
            SenderName = "FPT Bot",
            SenderEmail = "",
            SenderType = "bot",
            Message = _chatbotService.GetWelcomeMessage(dto.UserName),
            CreatedAt = DateTime.UtcNow.AddSeconds(1),
            IsRead = false
        };

        _context.ChatMessages.Add(welcomeMessage);

        // Nếu tin nhắn đầu tiên không phải greeting, bot trả lời luôn
        if (!dto.InitialMessage.ToLower().Contains("xin chào") &&
            !dto.InitialMessage.ToLower().Contains("hello") &&
            !dto.InitialMessage.ToLower().Contains("hi"))
        {
            var autoReply = _chatbotService.GetAutoReply(dto.InitialMessage);
            if (autoReply != null)
            {
                var replyMessage = new ChatMessage
                {
                    SessionId = sessionId,
                    UserId = null,
                    SenderName = "FPT Bot",
                    SenderEmail = "",
                    SenderType = "bot",
                    Message = autoReply,
                    CreatedAt = DateTime.UtcNow.AddSeconds(2),
                    IsRead = false
                };

                _context.ChatMessages.Add(replyMessage);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("New chat session created with bot: {SessionId}", sessionId);

        return await GetChatSession(sessionId)
            ?? throw new InvalidOperationException("Cannot retrieve created session");
    }

    public async Task<ChatSessionDto?> GetChatSession(string sessionId)
    {
        var session = await _context.ChatSessions
            .Include(s => s.User)
            .Include(s => s.AssignedStaff)
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return null;

        var lastMessage = session.Messages
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        return new ChatSessionDto
        {
            Id = session.Id,
            SessionId = session.SessionId,
            UserName = session.UserName ?? session.User?.Name,
            UserEmail = session.UserEmail ?? session.User?.Email,
            UserPhone = session.UserPhone ?? session.User?.Phone,
            AssignedStaffName = session.AssignedStaff?.Name,
            Status = session.Status,
            CreatedAt = session.CreatedAt,
            LastMessageAt = session.LastMessageAt,
            UnreadCount = session.Messages.Count(m => !m.IsRead && m.SenderType == "user"),
            LastMessage = lastMessage != null ? new ChatMessageDto
            {
                Id = lastMessage.Id,
                SessionId = lastMessage.SessionId,
                SenderName = lastMessage.SenderName,
                SenderType = lastMessage.SenderType,
                Message = lastMessage.Message,
                CreatedAt = lastMessage.CreatedAt,
                IsRead = lastMessage.IsRead
            } : null
        };
    }

    public async Task<List<ChatSessionDto>> GetAllChatSessions(string? status = null)
    {
        var query = _context.ChatSessions
            .Include(s => s.User)
            .Include(s => s.AssignedStaff)
            .Include(s => s.Messages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var sessions = await query
            .OrderByDescending(s => s.LastMessageAt ?? s.CreatedAt)
            .ToListAsync();

        return sessions.Select(MapToDto).ToList();
    }

    public async Task<List<ChatSessionDto>> GetUserChatSessions(int userId)
    {
        var sessions = await _context.ChatSessions
            .Include(s => s.User)
            .Include(s => s.AssignedStaff)
            .Include(s => s.Messages)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.LastMessageAt ?? s.CreatedAt)
            .ToListAsync();

        return sessions.Select(MapToDto).ToList();
    }

    public async Task<List<ChatMessageDto>> GetChatMessages(string sessionId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            SessionId = m.SessionId,
            SenderName = m.SenderName,
            SenderType = m.SenderType,
            Message = m.Message,
            CreatedAt = m.CreatedAt,
            IsRead = m.IsRead
        }).ToList();
    }

    public async Task<bool> CloseChatSession(string sessionId, int? staffId)
    {
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null) return false;

        session.Status = "closed";
        session.ClosedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        if (staffId.HasValue)
        {
            _logger.LogInformation("Chat session {SessionId} closed by staff {StaffId}", sessionId, staffId.Value);
        }
        else
        {
            _logger.LogInformation("Chat session {SessionId} closed by user", sessionId);
        }
        
        return true;
    }

    private ChatSessionDto MapToDto(ChatSession session)
    {
        var lastMessage = session.Messages
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        return new ChatSessionDto
        {
            Id = session.Id,
            SessionId = session.SessionId,
            UserName = session.UserName ?? session.User?.Name,
            UserEmail = session.UserEmail ?? session.User?.Email,
            UserPhone = session.UserPhone ?? session.User?.Phone,
            AssignedStaffName = session.AssignedStaff?.Name,
            Status = session.Status,
            CreatedAt = session.CreatedAt,
            LastMessageAt = session.LastMessageAt,
            UnreadCount = session.Messages.Count(m => !m.IsRead && m.SenderType == "user"),
            LastMessage = lastMessage != null ? new ChatMessageDto
            {
                Id = lastMessage.Id,
                SessionId = lastMessage.SessionId,
                SenderName = lastMessage.SenderName,
                SenderType = lastMessage.SenderType,
                Message = lastMessage.Message,
                CreatedAt = lastMessage.CreatedAt,
                IsRead = lastMessage.IsRead
            } : null
        };
    }
}