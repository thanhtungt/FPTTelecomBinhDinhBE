using FPTTelecomBE.DTOs.JobPosting;

namespace FPTTelecomBE.Services;

public interface IJobPostingService
{
    Task<JobPostingResponseDto> CreateJobPosting(CreateJobPostingDto dto, int userId);
    Task<JobPostingResponseDto?> GetJobPostingById(int id);
    Task<List<JobPostingResponseDto>> GetAllJobPostings(string? status = null);
    Task<List<JobPostingResponseDto>> GetActiveJobPostings();
    Task<JobPostingResponseDto?> UpdateJobPosting(int id, UpdateJobPostingDto dto, int userId);
    Task<bool> DeleteJobPosting(int id, int userId);
    Task<JobPostingResponseDto?> UpdateJobPostingStatus(int id, string status, int userId);
}