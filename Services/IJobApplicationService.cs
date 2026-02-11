using FPTTelecomBE.DTOs.JobApplication;

namespace FPTTelecomBE.Services;

public interface IJobApplicationService
{
    Task<JobApplicationResponseDto> CreateJobApplication(CreateJobApplicationDto dto);
    Task<JobApplicationResponseDto?> GetJobApplicationById(int id);
    Task<List<JobApplicationResponseDto>> GetAllJobApplications(string? status = null);
    Task<List<JobApplicationResponseDto>> GetJobApplicationsByJobPosting(int jobPostingId);
    Task<JobApplicationResponseDto?> UpdateJobApplicationStatus(int id, UpdateJobApplicationStatusDto dto, int reviewerId);
    Task<bool> DeleteJobApplication(int id);
}