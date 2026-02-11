using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.JobPosting;
using FPTTelecomBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Services;

public class JobPostingService : IJobPostingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<JobPostingService> _logger;

    public JobPostingService(AppDbContext context, ILogger<JobPostingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JobPostingResponseDto> CreateJobPosting(CreateJobPostingDto dto, int userId)
    {
        var jobPosting = new JobPosting
        {
            Title = dto.Title,
            Description = dto.Description,
            Position = dto.Position,
            Department = dto.Department,
            Location = dto.Location,
            EmploymentType = dto.EmploymentType,
            ExperienceLevel = dto.ExperienceLevel,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            Requirements = dto.Requirements,
            Benefits = dto.Benefits,
            NumberOfPositions = dto.NumberOfPositions,
            ApplicationDeadline = dto.ApplicationDeadline,
            Status = "draft",
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.JobPostings.Add(jobPosting);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tin tuyển dụng được tạo: {JobId} - {Title} bởi user {UserId}",
            jobPosting.Id, jobPosting.Title, userId);

        return await GetJobPostingById(jobPosting.Id)
            ?? throw new InvalidOperationException("Không thể lấy thông tin tin tuyển dụng vừa tạo");
    }

    public async Task<JobPostingResponseDto?> GetJobPostingById(int id)
    {
        var jobPosting = await _context.JobPostings
            .Include(j => j.CreatedBy)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jobPosting == null)
            return null;

        return MapToResponseDto(jobPosting);
    }

    public async Task<List<JobPostingResponseDto>> GetAllJobPostings(string? status = null)
    {
        var query = _context.JobPostings
            .Include(j => j.CreatedBy)
            .Include(j => j.Applications)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(j => j.Status == status);
        }

        var jobPostings = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return jobPostings.Select(MapToResponseDto).ToList();
    }

    public async Task<List<JobPostingResponseDto>> GetActiveJobPostings()
    {
        var now = DateTime.UtcNow;
        var jobPostings = await _context.JobPostings
            .Include(j => j.CreatedBy)
            .Include(j => j.Applications)
            .Where(j => j.Status == "active" &&
                       (j.ApplicationDeadline == null || j.ApplicationDeadline > now))
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return jobPostings.Select(MapToResponseDto).ToList();
    }

    public async Task<JobPostingResponseDto?> UpdateJobPosting(int id, UpdateJobPostingDto dto, int userId)
    {
        var jobPosting = await _context.JobPostings.FindAsync(id);
        if (jobPosting == null)
            return null;

        // Kiểm tra quyền (chỉ người tạo hoặc Admin mới sửa được)
        var user = await _context.Users.FindAsync(userId);
        if (jobPosting.CreatedByUserId != userId && user?.Role != "Admin")
        {
            _logger.LogWarning("User {UserId} cố gắng cập nhật tin tuyển dụng {JobId} không có quyền", userId, id);
            throw new UnauthorizedAccessException("Bạn không có quyền cập nhật tin tuyển dụng này");
        }

        jobPosting.Title = dto.Title;
        jobPosting.Description = dto.Description;
        jobPosting.Position = dto.Position;
        jobPosting.Department = dto.Department;
        jobPosting.Location = dto.Location;
        jobPosting.EmploymentType = dto.EmploymentType;
        jobPosting.ExperienceLevel = dto.ExperienceLevel;
        jobPosting.SalaryMin = dto.SalaryMin;
        jobPosting.SalaryMax = dto.SalaryMax;
        jobPosting.Requirements = dto.Requirements;
        jobPosting.Benefits = dto.Benefits;
        jobPosting.NumberOfPositions = dto.NumberOfPositions;
        jobPosting.ApplicationDeadline = dto.ApplicationDeadline;
        jobPosting.Status = dto.Status;
        jobPosting.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Tin tuyển dụng được cập nhật: {JobId} bởi user {UserId}", id, userId);

        return await GetJobPostingById(id);
    }

    public async Task<bool> DeleteJobPosting(int id, int userId)
    {
        var jobPosting = await _context.JobPostings
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jobPosting == null)
            return false;

        // Kiểm tra quyền
        var user = await _context.Users.FindAsync(userId);
        if (jobPosting.CreatedByUserId != userId && user?.Role != "Admin")
        {
            _logger.LogWarning("User {UserId} cố gắng xóa tin tuyển dụng {JobId} không có quyền", userId, id);
            throw new UnauthorizedAccessException("Bạn không có quyền xóa tin tuyển dụng này");
        }

        // Kiểm tra xem có hồ sơ ứng tuyển không
        if (jobPosting.Applications?.Any() == true)
        {
            _logger.LogWarning("Cố gắng xóa tin tuyển dụng {JobId} đã có người ứng tuyển", id);
            throw new InvalidOperationException("Không thể xóa tin tuyển dụng đã có người ứng tuyển");
        }

        _context.JobPostings.Remove(jobPosting);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tin tuyển dụng đã xóa: {JobId} bởi user {UserId}", id, userId);

        return true;
    }

    public async Task<JobPostingResponseDto?> UpdateJobPostingStatus(int id, string status, int userId)
    {
        var jobPosting = await _context.JobPostings.FindAsync(id);
        if (jobPosting == null)
            return null;

        // Kiểm tra quyền
        var user = await _context.Users.FindAsync(userId);
        if (jobPosting.CreatedByUserId != userId && user?.Role != "Admin")
        {
            _logger.LogWarning("User {UserId} cố gắng cập nhật trạng thái tin tuyển dụng {JobId} không có quyền", userId, id);
            throw new UnauthorizedAccessException("Bạn không có quyền cập nhật trạng thái tin tuyển dụng này");
        }

        // Validate trạng thái
        var validStatuses = new[] { "draft", "active", "closed", "expired" };
        if (!validStatuses.Contains(status))
        {
            throw new ArgumentException($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validStatuses)}");
        }

        jobPosting.Status = status;
        jobPosting.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Trạng thái tin tuyển dụng {JobId} được cập nhật thành {Status} bởi user {UserId}",
            id, status, userId);

        return await GetJobPostingById(id);
    }

    private JobPostingResponseDto MapToResponseDto(JobPosting j)
    {
        return new JobPostingResponseDto
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            Position = j.Position,
            Department = j.Department,
            Location = j.Location,
            EmploymentType = j.EmploymentType,
            ExperienceLevel = j.ExperienceLevel,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            SalaryCurrency = j.SalaryCurrency,
            Requirements = j.Requirements,
            Benefits = j.Benefits,
            NumberOfPositions = j.NumberOfPositions,
            ApplicationDeadline = j.ApplicationDeadline,
            Status = j.Status,
            CreatedByUserId = j.CreatedByUserId,
            CreatedByUserName = j.CreatedBy?.Name,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt,
            ApplicationCount = j.Applications?.Count ?? 0
        };
    }
}