using FPTTelecomBE.DTOs.JobPosting;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobPostingsController : ControllerBase
{
    private readonly IJobPostingService _jobPostingService;
    private readonly ILogger<JobPostingsController> _logger;

    public JobPostingsController(IJobPostingService jobPostingService, ILogger<JobPostingsController> logger)
    {
        _jobPostingService = jobPostingService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo tin tuyển dụng mới (Admin/Staff)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> CreateJobPosting([FromBody] CreateJobPostingDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobPostingService.CreateJobPosting(dto, userId);

            _logger.LogInformation("Tin tuyển dụng được tạo: {JobId} - {Title} bởi user {UserId}",
                result.Id, result.Title, userId);

            return CreatedAtAction(nameof(GetJobPostingById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo tin tuyển dụng");
            return StatusCode(500, new { message = "Lỗi khi tạo tin tuyển dụng" });
        }
    }

    /// <summary>
    /// Lấy chi tiết tin tuyển dụng theo ID (Public)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobPostingById(int id)
    {
        var jobPosting = await _jobPostingService.GetJobPostingById(id);
        if (jobPosting == null)
            return NotFound(new { message = "Không tìm thấy tin tuyển dụng" });

        return Ok(jobPosting);
    }

    /// <summary>
    /// Lấy tất cả tin tuyển dụng (Public - có thể filter theo status)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllJobPostings([FromQuery] string? status = null)
    {
        var jobPostings = await _jobPostingService.GetAllJobPostings(status);
        _logger.LogInformation("Đã lấy {Count} tin tuyển dụng", jobPostings.Count);
        return Ok(jobPostings);
    }

    /// <summary>
    /// Lấy danh sách tin tuyển dụng đang mở (Public)
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveJobPostings()
    {
        var jobPostings = await _jobPostingService.GetActiveJobPostings();
        _logger.LogInformation("Đã lấy {Count} tin tuyển dụng đang mở", jobPostings.Count);
        return Ok(jobPostings);
    }

    /// <summary>
    /// Cập nhật tin tuyển dụng (Admin/Staff/Creator)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateJobPosting(int id, [FromBody] UpdateJobPostingDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobPostingService.UpdateJobPosting(id, dto, userId);

            if (result == null)
                return NotFound(new { message = "Không tìm thấy tin tuyển dụng" });

            _logger.LogInformation("Tin tuyển dụng {JobId} được cập nhật bởi user {UserId}", id, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Cố gắng cập nhật tin tuyển dụng {JobId} không có quyền", id);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật tin tuyển dụng {JobId}", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật tin tuyển dụng" });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái tin tuyển dụng (Admin/Staff/Creator)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateJobPostingStatus(int id, [FromBody] UpdateJobPostingStatusDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobPostingService.UpdateJobPostingStatus(id, dto.Status, userId);

            if (result == null)
                return NotFound(new { message = "Không tìm thấy tin tuyển dụng" });

            _logger.LogInformation("Trạng thái tin tuyển dụng {JobId} được cập nhật thành {Status} bởi user {UserId}",
                id, dto.Status, userId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Cố gắng cập nhật trạng thái tin tuyển dụng {JobId} không có quyền", id);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Trạng thái không hợp lệ cho tin tuyển dụng {JobId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật trạng thái tin tuyển dụng {JobId}", id);
            return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái tin tuyển dụng" });
        }
    }

    /// <summary>
    /// Xóa tin tuyển dụng (Admin/Creator)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> DeleteJobPosting(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobPostingService.DeleteJobPosting(id, userId);

            if (!result)
                return NotFound(new { message = "Không tìm thấy tin tuyển dụng" });

            _logger.LogInformation("Tin tuyển dụng {JobId} đã xóa bởi user {UserId}", id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Cố gắng xóa tin tuyển dụng {JobId} không có quyền", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Không thể xóa tin tuyển dụng {JobId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa tin tuyển dụng {JobId}", id);
            return StatusCode(500, new { message = "Lỗi khi xóa tin tuyển dụng" });
        }
    }
}