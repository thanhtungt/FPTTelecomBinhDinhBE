using AutoMapper;
using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Registration;
using FPTTelecomBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Services;

public class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(AppDbContext context, IMapper mapper, ILogger<RegistrationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RegistrationResponseDto> CreateRegistration(CreateRegistrationDto dto)
    {
        var registration = new Registration
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Phone = dto.Phone,
            Address = dto.Address,
            PackageId = dto.PackageId,
            Note = dto.Note,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        // Auto-assign staff (optional - round-robin hoặc random)
        await AutoAssignStaff(registration.Id);

        return await GetRegistrationById(registration.Id) ?? throw new Exception("Failed to create registration");
    }

    public async Task<RegistrationResponseDto?> GetRegistrationById(int id)
    {
        var registration = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (registration == null) return null;

        return MapToResponseDto(registration);
    }

    public async Task<List<RegistrationResponseDto>> GetAllRegistrations()
    {
        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return registrations.Select(MapToResponseDto).ToList();
    }

    public async Task<List<RegistrationResponseDto>> GetRegistrationsByStatus(string status)
    {
        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return registrations.Select(MapToResponseDto).ToList();
    }

    public async Task<List<RegistrationResponseDto>> GetUserRegistrations(int userId)
    {
        var registrations = await _context.Registrations
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return registrations.Select(MapToResponseDto).ToList();
    }

    public async Task<RegistrationResponseDto?> UpdateStatus(int id, UpdateRegistrationStatusDto dto)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null) return null;

        // Validate status transition
        ValidateStatusTransition(registration.Status, dto.Status);

        registration.Status = dto.Status;

        if (dto.AssignedStaffId.HasValue)
        {
            registration.AssignedStaffId = dto.AssignedStaffId;
        }

        registration.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetRegistrationById(id);
    }

    public async Task<RegistrationResponseDto?> AssignStaff(int registrationId, int staffId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);
        if (registration == null) return null;

        // Verify staff exists and has Staff/Admin role
        var staff = await _context.Users.FindAsync(staffId);
        if (staff == null || (staff.Role != "Staff" && staff.Role != "Admin"))
        {
            _logger.LogWarning("Invalid staff assignment: StaffId {StaffId} not found or not Staff/Admin", staffId);
            return null;
        }

        registration.AssignedStaffId = staffId;
        registration.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetRegistrationById(registrationId);
    }

    public async Task<string> DeleteRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null) return "not_found";

        // Không cho phép xóa đơn đã installed hoặc done
        if (registration.Status == "installed" || registration.Status == "done")
        {
            _logger.LogWarning("Attempted to delete registration {RegId} with status {Status}", id, registration.Status);
            return "cannot_delete";
        }

        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync();

        return "success";
    }

    // Helper methods
    private RegistrationResponseDto MapToResponseDto(Registration r)
    {
        return new RegistrationResponseDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User?.Name,
            FullName = r.FullName,
            Phone = r.Phone,
            Address = r.Address,
            PackageId = r.PackageId,
            PackageName = r.Package?.Name ?? "",
            PackagePrice = r.Package?.PriceMonthly ?? 0,
            Note = r.Note,
            Status = r.Status,
            AssignedStaffId = r.AssignedStaffId,
            AssignedStaffName = r.AssignedStaff?.Name,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    private async Task AutoAssignStaff(int registrationId)
    {
        try
        {
            // Get available staff (Staff or Admin role)
            var staffList = await _context.Users
                .Where(u => u.Role == "Staff" || u.Role == "Admin")
                .ToListAsync();

            if (!staffList.Any())
            {
                _logger.LogWarning("No staff available for auto-assignment");
                return;
            }

            // Round-robin: Find staff with least assigned pending registrations
            var staffWorkload = new Dictionary<int, int>();

            foreach (var staff in staffList)
            {
                var pendingCount = await _context.Registrations
                    .CountAsync(r => r.AssignedStaffId == staff.Id && r.Status == "pending");
                staffWorkload[staff.Id] = pendingCount;
            }

            var selectedStaffId = staffWorkload.OrderBy(x => x.Value).First().Key;

            var registration = await _context.Registrations.FindAsync(registrationId);
            if (registration != null)
            {
                registration.AssignedStaffId = selectedStaffId;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Auto-assigned registration {RegId} to staff {StaffId}", registrationId, selectedStaffId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in auto-assigning staff for registration {RegId}", registrationId);
        }
    }

    private void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        // Define valid transitions
        var validTransitions = new Dictionary<string, List<string>>
        {
            { "pending", new List<string> { "contacting", "cancelled" } },
            { "contacting", new List<string> { "need_survey", "cancelled" } },
            { "need_survey", new List<string> { "surveyed", "cancelled" } },
            { "surveyed", new List<string> { "contract_signed", "cancelled" } },
            { "contract_signed", new List<string> { "installation_scheduled", "cancelled" } },
            { "installation_scheduled", new List<string> { "installed", "cancelled" } },
            { "installed", new List<string> { "done" } },
            { "cancelled", new List<string>() }, // Cannot transition from cancelled
            { "done", new List<string>() } // Cannot transition from done
        };

        if (!validTransitions.ContainsKey(currentStatus))
        {
            throw new InvalidOperationException($"Trạng thái hiện tại không hợp lệ: {currentStatus}");
        }

        if (!validTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Không thể chuyển từ trạng thái '{currentStatus}' sang '{newStatus}'. " +
                $"Trạng thái hợp lệ: {string.Join(", ", validTransitions[currentStatus])}"
            );
        }
    }
}