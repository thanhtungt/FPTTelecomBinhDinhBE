using FPTTelecomBE.DTOs.Chat;

namespace FPTTelecomBE.Services;

public interface IChatService
{
    Task<ChatSessionDto> StartChatSession(StartChatDto dto, int? userId);
    Task<ChatSessionDto?> GetChatSession(string sessionId);
    Task<List<ChatSessionDto>> GetAllChatSessions(string? status = null);
    Task<List<ChatSessionDto>> GetUserChatSessions(int userId);
    Task<List<ChatMessageDto>> GetChatMessages(string sessionId);
    Task<bool> CloseChatSession(string sessionId, int? staffId);
}