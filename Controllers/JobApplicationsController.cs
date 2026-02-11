using FPTTelecomBE.DTOs.JobApplication;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _jobApplicationService;
    private readonly ILogger<JobApplicationsController> _logger;

    public JobApplicationsController(
        IJobApplicationService jobApplicationService,
        ILogger<JobApplicationsController> logger)
    {
        _jobApplicationService = jobApplicationService;
        _logger = logger;
    }

    /// <summary>
    /// Nộp hồ sơ ứng tuyển (Public)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateJobApplication([FromBody] CreateJobApplicationDto dto)
    {
        try
        {
            var result = await _jobApplicationService.CreateJobApplication(dto);

            _logger.LogInformation("Hồ sơ ứng tuyển được tạo: {AppId} - {FullName}",
                result.Id, result.FullName);

            return CreatedAtAction(nameof(GetJobApplicationById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Không thể tạo hồ sơ ứng tuyển");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo hồ sơ ứng tuyển");
            return StatusCode(500, new { message = "Lỗi khi nộp hồ sơ ứng tuyển" });
        }
    }

    /// <summary>
    /// Lấy chi tiết hồ sơ ứng tuyển theo ID (Admin/Staff)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetJobApplicationById(int id)
    {
        var application = await _jobApplicationService.GetJobApplicationById(id);
        if (application == null)
            return NotFound(new { message = "Không tìm thấy hồ sơ ứng tuyển" });

        return Ok(application);
    }

    /// <summary>
    /// Lấy tất cả hồ sơ ứng tuyển (Admin/Staff - có thể filter theo status)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllJobApplications([FromQuery] string? status = null)
    {
        var applications = await _jobApplicationService.GetAllJobApplications(status);
        _logger.LogInformation("Đã lấy {Count} hồ sơ ứng tuyển", applications.Count);
        return Ok(applications);
    }

    /// <summary>
    /// Lấy hồ sơ ứng tuyển theo tin tuyển dụng (Admin/Staff)
    /// </summary>
    [HttpGet("by-job/{jobPostingId}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetJobApplicationsByJobPosting(int jobPostingId)
    {
        var applications = await _jobApplicationService.GetJobApplicationsByJobPosting(jobPostingId);
        _logger.LogInformation("Đã lấy {Count} hồ sơ ứng tuyển cho tin {JobId}",
            applications.Count, jobPostingId);
        return Ok(applications);
    }

    /// <summary>
    /// Cập nhật trạng thái hồ sơ ứng tuyển (Admin/Staff)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateJobApplicationStatus(
        int id, [FromBody] UpdateJobApplicationStatusDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobApplicationService.UpdateJobApplicationStatus(id, dto, userId);

            if (result == null)
                return NotFound(new { message = "Không tìm thấy hồ sơ ứng tuyển" });

            _logger.LogInformation("Trạng thái hồ sơ {AppId} được cập nhật thành {Status} bởi user {UserId}",
                id, dto.Status, userId);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Trạng thái không hợp lệ cho hồ sơ {AppId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật trạng thái hồ sơ {AppId}", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái hồ sơ" });
        }
    }

    /// <summary>
    /// Xóa hồ sơ ứng tuyển (Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteJobApplication(int id)
    {
        try
        {
            var result = await _jobApplicationService.DeleteJobApplication(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy hồ sơ ứng tuyển" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation("Hồ sơ ứng tuyển {AppId} đã xóa bởi user {UserId}", id, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa hồ sơ ứng tuyển {AppId}", id);
            return StatusCode(500, new { message = "Lỗi khi xóa hồ sơ ứng tuyển" });
        }
    }
}