using FPTTelecomBE.DTOs.Chat;
using FPTTelecomBE.Hubs;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IChatService chatService, ILogger<ChatController> logger, IHubContext<ChatHub> hubContext)
    {
        _chatService = chatService;
        _logger = logger;
        _hubContext = hubContext;
    }

    // Start chat session (Public - có thể không cần login)
    [HttpPost("start")]
    public async Task<IActionResult> StartChat([FromBody] StartChatDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            var session = await _chatService.StartChatSession(dto, userId);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting chat session");
            return StatusCode(500, new { message = "Không thể bắt đầu chat" });
        }
    }

    // Get chat session by sessionId (Public)
    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        var session = await _chatService.GetChatSession(sessionId);
        if (session == null)
            return NotFound(new { message = "Không tìm thấy chat session" });

        return Ok(session);
    }

    // Get all chat sessions (Admin/Staff only)
    [HttpGet("sessions")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllSessions([FromQuery] string? status = null)
    {
        var sessions = await _chatService.GetAllChatSessions(status);
        return Ok(sessions);
    }

    // Get user's chat sessions (User only)
    [HttpGet("my-sessions")]
    [Authorize]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var sessions = await _chatService.GetUserChatSessions(userId);
        return Ok(sessions);
    }

    // Get messages of a session (Public với sessionId)
    [HttpGet("session/{sessionId}/messages")]
    public async Task<IActionResult> GetMessages(string sessionId)
    {
        var messages = await _chatService.GetChatMessages(sessionId);
        return Ok(messages);
    }

    // Close chat session (Admin/Staff only)
    [HttpPut("session/{sessionId}/close")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> CloseSession(string sessionId)
    {
        try
        {
            var staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _chatService.CloseChatSession(sessionId, staffId);

            if (!result)
                return NotFound(new { message = "Không tìm thấy chat session" });

            // Broadcast to session group that it's closed
            await _hubContext.Clients.Group(sessionId).SendAsync("SessionClosed", new
            {
                SessionId = sessionId,
                ClosedBy = "staff",
                ClosedAt = DateTime.UtcNow
            });

            return Ok(new { message = "Chat session đã đóng" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing chat session");
            return StatusCode(500, new { message = "Không thể đóng chat session" });
        }
    }

    // Close chat session by user (Public - user can close their own session)
    [HttpPut("session/{sessionId}/close-by-user")]
    public async Task<IActionResult> CloseSessionByUser(string sessionId)
    {
        try
        {
            var result = await _chatService.CloseChatSession(sessionId, null);

            if (!result)
                return NotFound(new { message = "Không tìm thấy chat session" });

            // Broadcast to session group that it's closed
            await _hubContext.Clients.Group(sessionId).SendAsync("SessionClosed", new
            {
                SessionId = sessionId,
                ClosedBy = "user",
                ClosedAt = DateTime.UtcNow
            });

            // Also notify admin/staff group
            await _hubContext.Clients.Group("AdminStaff").SendAsync("SessionClosedByUser", new
            {
                SessionId = sessionId,
                ClosedAt = DateTime.UtcNow
            });

            return Ok(new { message = "Chat session đã đóng" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing chat session");
            return StatusCode(500, new { message = "Không thể đóng chat session" });
        }
    }
}