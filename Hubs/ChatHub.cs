using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Chat;
using FPTTelecomBE.Models;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FPTTelecomBE.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _context;
    private readonly ILogger<ChatHub> _logger;
    private readonly IChatbotService _chatbotService;

    public ChatHub(AppDbContext context, ILogger<ChatHub> logger, IChatbotService chatbotService)
    {
        _context = context;
        _logger = logger;
        _chatbotService = chatbotService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        // Nếu là admin/staff, join vào group để nhận thông báo chat mới
        if (userRole == "Admin" || userRole == "Staff")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminStaff");
            _logger.LogInformation("Admin/Staff {UserId} connected to chat", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == "Admin" || userRole == "Staff")
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminStaff");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChatSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogInformation("Connection {ConnectionId} joined session {SessionId}",
            Context.ConnectionId, sessionId);
    }

    public async Task LeaveChatSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }

    public async Task SendMessage(SendMessageDto dto)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            var session = await _context.ChatSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionId == dto.SessionId);

            if (session == null)
            {
                throw new HubException("Chat session không tồn tại");
            }

            // Xác định sender type
            string senderType = "user";
            string senderName = dto.SenderName ?? "Anonymous";

            if (userId != null && (userRole == "Admin" || userRole == "Staff"))
            {
                senderType = userRole.ToLower();
                senderName = userName ?? "Admin/Staff";
            }
            else if (userId != null)
            {
                senderName = userName ?? session.UserName ?? "User";
            }

            var message = new ChatMessage
            {
                SessionId = dto.SessionId,
                UserId = userId != null ? int.Parse(userId) : null,
                SenderName = senderName,
                SenderEmail = dto.SenderEmail ?? "",
                SenderType = senderType,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            session.LastMessageAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                Id = message.Id,
                SessionId = message.SessionId,
                SenderName = message.SenderName,
                SenderType = message.SenderType,
                Message = message.Message,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead
            };

            // Send message to session group
            await Clients.Group(dto.SessionId).SendAsync("ReceiveMessage", messageDto);

            // Xử lý logic bot và staff
            if (senderType == "user")
            {
                // Kiểm tra xem user có yêu cầu kết nối staff không
                bool isRequestingStaff = _chatbotService.IsRequestingStaff(dto.Message);

                if (isRequestingStaff && (session.Status == "bot" || session.Status == "waiting"))
                {
                    // Chuyển session sang waiting và thông báo admin/staff
                    session.Status = "waiting";
                    await _context.SaveChangesAsync();

                    // Bot thông báo đang kết nối
                    await Task.Delay(500);
                    var connectingMessage = new ChatMessage
                    {
                        SessionId = dto.SessionId,
                        UserId = null,
                        SenderName = "FPT Bot",
                        SenderEmail = "",
                        SenderType = "bot",
                        Message = "⏳ **Đang kết nối với tư vấn viên...**\n\n" +
                                  "Vui lòng chờ trong giây lát, tư vấn viên sẽ hỗ trợ bạn ngay! 😊\n\n" +
                                  "📞 Nếu cần gấp, vui lòng gọi: **1900 xxxx**",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    _context.ChatMessages.Add(connectingMessage);
                    await _context.SaveChangesAsync();

                    var connectingDto = new ChatMessageDto
                    {
                        Id = connectingMessage.Id,
                        SessionId = connectingMessage.SessionId,
                        SenderName = connectingMessage.SenderName,
                        SenderType = connectingMessage.SenderType,
                        Message = connectingMessage.Message,
                        CreatedAt = connectingMessage.CreatedAt,
                        IsRead = connectingMessage.IsRead
                    };

                    await Clients.Group(dto.SessionId).SendAsync("ReceiveMessage", connectingDto);

                    // Thông báo tới Admin/Staff
                    await Clients.Group("AdminStaff").SendAsync("NewStaffRequest", new
                    {
                        SessionId = dto.SessionId,
                        UserName = senderName,
                        UserEmail = session.UserEmail,
                        UserPhone = session.UserPhone,
                        Message = dto.Message,
                        CreatedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation("User requested staff connection for session {SessionId}", dto.SessionId);
                }
                else if (session.Status == "bot")
                {
                    // Vẫn đang chat với bot, bot tự động trả lời
                    await Task.Delay(800); // Giả lập typing...

                    var botReply = _chatbotService.GetAutoReply(dto.Message);

                    if (botReply != null)
                    {
                        var botMessage = new ChatMessage
                        {
                            SessionId = dto.SessionId,
                            UserId = null,
                            SenderName = "FPT Bot",
                            SenderEmail = "",
                            SenderType = "bot",
                            Message = botReply,
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false
                        };

                        _context.ChatMessages.Add(botMessage);
                        await _context.SaveChangesAsync();

                        var botMessageDto = new ChatMessageDto
                        {
                            Id = botMessage.Id,
                            SessionId = botMessage.SessionId,
                            SenderName = botMessage.SenderName,
                            SenderType = botMessage.SenderType,
                            Message = botMessage.Message,
                            CreatedAt = botMessage.CreatedAt,
                            IsRead = botMessage.IsRead
                        };

                        await Clients.Group(dto.SessionId).SendAsync("ReceiveMessage", botMessageDto);
                    }
                }
                else if (session.Status == "waiting")
                {
                    // Đang chờ staff, không làm gì thêm
                    // Có thể thông báo lại cho admin/staff
                    await Clients.Group("AdminStaff").SendAsync("UpdateUnreadCount", new
                    {
                        SessionId = dto.SessionId,
                        UnreadCount = await _context.ChatMessages
                            .CountAsync(m => m.SessionId == dto.SessionId && !m.IsRead && m.SenderType == "user")
                    });
                }
            }
            else if (senderType == "admin" || senderType == "staff")
            {
                // Staff/Admin trả lời, chuyển session sang active
                if (session.Status == "waiting" || session.Status == "bot")
                {
                    session.Status = "active";
                    if (userId != null)
                    {
                        session.AssignedStaffId = int.Parse(userId);
                    }
                    await _context.SaveChangesAsync();

                    // Thông báo staff đã kết nối
                    await Clients.Group(dto.SessionId).SendAsync("StaffConnected", new
                    {
                        StaffName = senderName,
                        ConnectedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation("Staff {StaffName} connected to session {SessionId}", senderName, dto.SessionId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            throw new HubException("Không thể gửi tin nhắn");
        }
    }

    public async Task AssignToSession(string sessionId)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && userRole != "Staff")
            {
                throw new HubException("Chỉ Admin/Staff mới có thể nhận chat");
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null && userId != null)
            {
                session.AssignedStaffId = int.Parse(userId);
                session.Status = "active";
                await _context.SaveChangesAsync();

                // Thông báo cho user
                var notificationMessage = new ChatMessage
                {
                    SessionId = sessionId,
                    UserId = null,
                    SenderName = "Hệ thống",
                    SenderEmail = "",
                    SenderType = "system",
                    Message = $"✅ Tư vấn viên **{userName}** đã kết nối và sẵn sàng hỗ trợ bạn!",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                _context.ChatMessages.Add(notificationMessage);
                await _context.SaveChangesAsync();

                var notificationDto = new ChatMessageDto
                {
                    Id = notificationMessage.Id,
                    SessionId = notificationMessage.SessionId,
                    SenderName = notificationMessage.SenderName,
                    SenderType = notificationMessage.SenderType,
                    Message = notificationMessage.Message,
                    CreatedAt = notificationMessage.CreatedAt,
                    IsRead = notificationMessage.IsRead
                };

                await Clients.Group(sessionId).SendAsync("ReceiveMessage", notificationDto);
                await Clients.Group(sessionId).SendAsync("StaffAssigned", new
                {
                    StaffName = userName,
                    AssignedAt = DateTime.UtcNow
                });

                _logger.LogInformation("Staff {StaffName} assigned to session {SessionId}", userName, sessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning to session");
            throw new HubException("Không thể nhận chat");
        }
    }

    public async Task MarkAsRead(string sessionId)
    {
        try
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == sessionId && !m.IsRead)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();

            await Clients.Group(sessionId).SendAsync("MessagesMarkedAsRead", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read");
        }
    }
}