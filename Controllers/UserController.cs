using FPTTelecomBE.DTOs.User;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? role = null)
    {
        try
        {
            var users = await _userService.GetAllUsers(role);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, new { message = "Lỗi khi lấy danh sách users" });
        }
    }

    /// <summary>
    /// Lấy thông tin user theo ID (Admin hoặc chính user đó)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Chỉ Admin hoặc chính user đó mới xem được
            if (userRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, new { message = "Lỗi khi lấy thông tin user" });
        }
    }

    /// <summary>
    /// Lấy thông tin profile của user hiện tại
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user profile");
            return StatusCode(500, new { message = "Lỗi khi lấy thông tin profile" });
        }
    }

    /// <summary>
    /// Tạo user mới (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _userService.CreateUser(dto);
            _logger.LogInformation("User created: {UserId} - {Name}", user.Id, user.Name);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid role when creating user");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User creation failed - duplicate data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "Lỗi khi tạo user" });
        }
    }

    /// <summary>
    /// Cập nhật thông tin user (Admin hoặc chính user đó)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Chỉ Admin hoặc chính user đó mới update được
            if (userRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userService.UpdateUser(id, dto);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            _logger.LogInformation("User updated: {UserId} by {CurrentUserId}", id, currentUserId);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User update failed - duplicate data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật user" });
        }
    }

    /// <summary>
    /// Cập nhật role của user (Admin only)
    /// </summary>
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.UpdateUserRole(id, dto.Role, adminId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            _logger.LogInformation("User {UserId} role updated to {Role} by admin {AdminId}",
                id, dto.Role, adminId);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid role when updating user role");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot update user role");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId} role", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật role" });
        }
    }

    /// <summary>
    /// Xóa user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _userService.DeleteUser(id, adminId);

            if (!result)
                return NotFound(new { message = "Không tìm thấy user" });

            _logger.LogInformation("User {UserId} deleted by admin {AdminId}", id, adminId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete user {UserId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, new { message = "Lỗi khi xóa user" });
        }
    }

    /// <summary>
    /// Đổi mật khẩu (User tự đổi)
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _userService.ChangePassword(userId, dto.CurrentPassword, dto.NewPassword);

            if (!result)
                return NotFound(new { message = "Không tìm thấy user" });

            _logger.LogInformation("Password changed for user {UserId}", userId);
            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Password change failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { message = "Lỗi khi đổi mật khẩu" });
        }
    }

    /// <summary>
    /// Lấy thống kê users (Admin only)
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserStatistics()
    {
        try
        {
            var totalUsers = await _userService.GetTotalUsersCount();
            var usersByRole = await _userService.GetUserStatsByRole();

            return Ok(new
            {
                TotalUsers = totalUsers,
                UsersByRole = usersByRole
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics");
            return StatusCode(500, new { message = "Lỗi khi lấy thống kê users" });
        }
    }
}