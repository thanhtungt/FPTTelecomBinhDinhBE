using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.JobApplication;
using FPTTelecomBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<JobApplicationService> _logger;
    private readonly IEmailService _emailService;

    public JobApplicationService(AppDbContext context, ILogger<JobApplicationService> logger, IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<JobApplicationResponseDto> CreateJobApplication(CreateJobApplicationDto dto)
    {
        // Kiểm tra tin tuyển dụng có tồn tại và đang active không
        var jobPosting = await _context.JobPostings.FindAsync(dto.JobPostingId);
        if (jobPosting == null)
        {
            throw new InvalidOperationException("Tin tuyển dụng không tồn tại");
        }

        if (jobPosting.Status != "active")
        {
            throw new InvalidOperationException("Tin tuyển dụng này không còn mở");
        }

        // Kiểm tra hạn nộp hồ sơ
        if (jobPosting.ApplicationDeadline.HasValue && jobPosting.ApplicationDeadline.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Đã hết hạn nộp hồ sơ");
        }

        // Kiểm tra xem đã ứng tuyển chưa (cùng email + cùng JobPostingId)
        var existingApplication = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.JobPostingId == dto.JobPostingId && a.Email == dto.Email);

        if (existingApplication != null)
        {
            throw new InvalidOperationException("Bạn đã ứng tuyển vị trí này rồi");
        }

        var jobApplication = new JobApplication
        {
            JobPostingId = dto.JobPostingId,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            CoverLetter = dto.CoverLetter,
            ResumeUrl = dto.ResumeUrl,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.JobApplications.Add(jobApplication);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hồ sơ ứng tuyển được tạo: {AppId} - {FullName} cho tin {JobId}",
            jobApplication.Id, jobApplication.FullName, dto.JobPostingId);

        var result = await GetJobApplicationById(jobApplication.Id)
            ?? throw new InvalidOperationException("Không thể lấy thông tin hồ sơ vừa tạo");

        // ← GỬI EMAIL CHO ADMIN 2
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendJobApplicationNotificationToAdminAsync(
                    applicationId: result.Id.ToString(),
                    candidateName: result.FullName,
                    email: result.Email,
                    phone: result.Phone,
                    position: result.JobPostingPosition ?? "",
                    jobTitle: result.JobPostingTitle ?? ""
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email thông báo ứng tuyển");
            }
        });

        return result;
    }
    public async Task<JobApplicationResponseDto?> GetJobApplicationById(int id)
    {
        var application = await _context.JobApplications
            .Include(a => a.JobPosting)
            .Include(a => a.ReviewedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
            return null;

        return MapToResponseDto(application);
    }

    public async Task<List<JobApplicationResponseDto>> GetAllJobApplications(string? status = null)
    {
        var query = _context.JobApplications
            .Include(a => a.JobPosting)
            .Include(a => a.ReviewedBy)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status == status);
        }

        var applications = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponseDto).ToList();
    }

    public async Task<List<JobApplicationResponseDto>> GetJobApplicationsByJobPosting(int jobPostingId)
    {
        var applications = await _context.JobApplications
            .Include(a => a.JobPosting)
            .Include(a => a.ReviewedBy)
            .Where(a => a.JobPostingId == jobPostingId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToResponseDto).ToList();
    }

    public async Task<JobApplicationResponseDto?> UpdateJobApplicationStatus(
        int id, UpdateJobApplicationStatusDto dto, int reviewerId)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application == null)
            return null;

        // Validate trạng thái
        var validStatuses = new[] { "pending", "reviewing", "interview", "accepted", "rejected" };
        if (!validStatuses.Contains(dto.Status))
        {
            throw new ArgumentException($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validStatuses)}");
        }

        application.Status = dto.Status;
        application.ReviewNote = dto.ReviewNote;
        application.ReviewedByUserId = reviewerId;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Hồ sơ ứng tuyển {AppId} được cập nhật trạng thái thành {Status} bởi user {ReviewerId}",
            id, dto.Status, reviewerId);

        return await GetJobApplicationById(id);
    }

    public async Task<bool> DeleteJobApplication(int id)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application == null)
            return false;

        _context.JobApplications.Remove(application);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hồ sơ ứng tuyển đã xóa: {AppId}", id);

        return true;
    }

    private JobApplicationResponseDto MapToResponseDto(JobApplication a)
    {
        return new JobApplicationResponseDto
        {
            Id = a.Id,
            JobPostingId = a.JobPostingId,
            JobPostingTitle = a.JobPosting?.Title,
            JobPostingPosition = a.JobPosting?.Position,
            FullName = a.FullName,
            Email = a.Email,
            Phone = a.Phone,
            Address = a.Address,
            CoverLetter = a.CoverLetter,
            ResumeUrl = a.ResumeUrl,
            Status = a.Status,
            ReviewedByUserId = a.ReviewedByUserId,
            ReviewedByUserName = a.ReviewedBy?.Name,
            ReviewNote = a.ReviewNote,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}