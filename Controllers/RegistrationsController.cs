using FPTTelecomBE.DTOs.Registration;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(IRegistrationService registrationService, ILogger<RegistrationsController> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    // POST: api/registrations (Public - Đăng ký nhanh)
    [HttpPost]
    public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationDto dto)
    {
        try
        {
            var result = await _registrationService.CreateRegistration(dto);
            _logger.LogInformation("New registration created: {Id} - {FullName} - {Phone}", result.Id, result.FullName, result.Phone);
            return CreatedAtAction(nameof(GetRegistrationById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating registration for {FullName}", dto.FullName);
            throw;
        }
    }

    // GET: api/registrations/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetRegistrationById(int id)
    {
        var registration = await _registrationService.GetRegistrationById(id);
        if (registration == null)
            return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // User chỉ xem được đơn của mình, Admin/Staff xem được tất cả
        if (userRole == "User" && registration.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access registration {RegId} without permission", userId, id);
            return Forbid();
        }

        return Ok(registration);
    }

    // GET: api/registrations (Admin/Staff only)
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllRegistrations([FromQuery] string? status = null)
    {
        var registrations = status != null
            ? await _registrationService.GetRegistrationsByStatus(status)
            : await _registrationService.GetAllRegistrations();

        _logger.LogInformation("Retrieved {Count} registrations", registrations.Count);
        return Ok(registrations);
    }

    // GET: api/registrations/my (User/Admin/Staff xem đơn của mình)
    [HttpGet("my")]
    [Authorize] // Cho phép tất cả user đã đăng nhập
    public async Task<IActionResult> GetMyRegistrations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogWarning("Invalid user ID claim in token");
            return BadRequest(new { message = "Token không hợp lệ" });
        }

        var registrations = await _registrationService.GetUserRegistrations(userId);
        _logger.LogInformation("User {UserId} retrieved {Count} registrations", userId, registrations.Count);
        return Ok(registrations);
    }

    // PUT: api/registrations/{id}/status (Admin/Staff only)
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRegistrationStatusDto dto)
    {
        try
        {
            var result = await _registrationService.UpdateStatus(id, dto);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            var staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation("Registration {RegId} status updated to {Status} by staff {StaffId}", id, dto.Status, staffId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid status transition for registration {RegId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating registration {RegId}", id);
            throw;
        }
    }

    // PUT: api/registrations/{id}/assign (Admin/Staff only - Assign staff manually)
    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> AssignStaff(int id, [FromBody] AssignStaffDto dto)
    {
        try
        {
            var result = await _registrationService.AssignStaff(id, dto.StaffId);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký hoặc nhân viên" });

            _logger.LogInformation("Registration {RegId} assigned to staff {StaffId}", id, dto.StaffId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning staff to registration {RegId}", id);
            throw;
        }
    }

    // DELETE: api/registrations/{id} (Admin/Staff only - Có kiểm tra trạng thái)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> DeleteRegistration(int id)
    {
        try
        {
            var result = await _registrationService.DeleteRegistration(id);

            if (result == "not_found")
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            if (result == "cannot_delete")
                return BadRequest(new { message = "Không thể xóa đơn đã hoàn thành hoặc đang lắp đặt" });

            var staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation("Registration {RegId} deleted by staff {StaffId}", id, staffId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting registration {RegId}", id);
            throw;
        }
    }
}