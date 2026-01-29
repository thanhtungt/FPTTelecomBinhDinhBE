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

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    // POST: api/registrations (Public - Đăng ký nhanh)
    [HttpPost]
    public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationDto dto)
    {
        var result = await _registrationService.CreateRegistration(dto);
        return CreatedAtAction(nameof(GetRegistrationById), new { id = result.Id }, result);
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

        // User chỉ xem được đơn của mình
        if (userRole == "User" && registration.UserId != userId)
            return Forbid();

        return Ok(registration);
    }

    // GET: api/registrations (Admin/Staff only)
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllRegistrations()
    {
        var registrations = await _registrationService.GetAllRegistrations();
        return Ok(registrations);
    }

    // GET: api/registrations/my (User xem đơn của mình)
    [HttpGet("my")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMyRegistrations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var registrations = await _registrationService.GetUserRegistrations(userId);
        return Ok(registrations);
    }

    // PUT: api/registrations/{id}/status (Admin/Staff only)
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRegistrationStatusDto dto)
    {
        var result = await _registrationService.UpdateStatus(id, dto);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

        return Ok(result);
    }

    // DELETE: api/registrations/{id} (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRegistration(int id)
    {
        var result = await _registrationService.DeleteRegistration(id);
        if (!result)
            return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

        return NoContent();
    }
}