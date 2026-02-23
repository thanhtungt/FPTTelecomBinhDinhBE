using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.User;
using FPTWifiBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        return MapToDto(user);
    }

    public async Task<List<UserDto>> GetAllUsers(string? role = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role == role);
        }

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> CreateUser(CreateUserDto dto)
    {
        // Validate role
        var validRoles = new[] { "User", "Admin", "Staff" };
        if (!validRoles.Contains(dto.Role))
        {
            throw new ArgumentException($"Role không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validRoles)}");
        }

        // Check email uniqueness (if provided)
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (existingEmail)
            {
                throw new InvalidOperationException("Email đã được sử dụng");
            }
        }

        // Check phone uniqueness
        var existingPhone = await _context.Users.AnyAsync(u => u.Phone == dto.Phone);
        if (existingPhone)
        {
            throw new InvalidOperationException("Số điện thoại đã được sử dụng");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created: {UserId} - {Name} with role {Role}",
            user.Id, user.Name, user.Role);

        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUser(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        // Check email uniqueness (if changed)
        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            var existingEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
            if (existingEmail)
            {
                throw new InvalidOperationException("Email đã được sử dụng");
            }
        }

        // Check phone uniqueness (if changed)
        if (dto.Phone != user.Phone)
        {
            var existingPhone = await _context.Users.AnyAsync(u => u.Phone == dto.Phone && u.Id != id);
            if (existingPhone)
            {
                throw new InvalidOperationException("Số điện thoại đã được sử dụng");
            }
        }

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Phone = dto.Phone;

        // Update password if provided
        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User updated: {UserId} - {Name}", user.Id, user.Name);

        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserRole(int id, string role, int adminId)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        // Validate role
        var validRoles = new[] { "User", "Admin", "Staff" };
        if (!validRoles.Contains(role))
        {
            throw new ArgumentException($"Role không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validRoles)}");
        }

        // Prevent self-demotion from Admin
        if (id == adminId && user.Role == "Admin" && role != "Admin")
        {
            throw new InvalidOperationException("Bạn không thể thay đổi role của chính mình");
        }

        var oldRole = user.Role;
        user.Role = role;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} role changed from {OldRole} to {NewRole} by admin {AdminId}",
            id, oldRole, role, adminId);

        return MapToDto(user);
    }

    public async Task<bool> DeleteUser(int id, int adminId)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        // Prevent self-deletion
        if (id == adminId)
        {
            throw new InvalidOperationException("Bạn không thể xóa tài khoản của chính mình");
        }

        // Check if user has related data
        var hasRegistrations = await _context.Registrations.AnyAsync(r => r.UserId == id);
        var hasJobPostings = await _context.JobPostings.AnyAsync(j => j.CreatedByUserId == id);
        var hasChatSessions = await _context.ChatSessions.AnyAsync(c => c.UserId == id || c.AssignedStaffId == id);

        if (hasRegistrations || hasJobPostings || hasChatSessions)
        {
            throw new InvalidOperationException(
                "Không thể xóa user vì còn dữ liệu liên quan (đơn đăng ký, tin tuyển dụng, hoặc chat sessions)");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted: {UserId} - {Name} by admin {AdminId}",
            id, user.Name, adminId);

        return true;
    }

    public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Mật khẩu hiện tại không đúng");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password changed for user {UserId}", userId);

        return true;
    }

    public async Task<int> GetTotalUsersCount()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetUserStatsByRole()
    {
        var stats = await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Role, x => x.Count);

        return stats;
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}