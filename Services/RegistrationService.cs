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

    public RegistrationService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

        return new RegistrationResponseDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            UserName = registration.User?.Name,
            FullName = registration.FullName,
            Phone = registration.Phone,
            Address = registration.Address,
            PackageId = registration.PackageId,
            PackageName = registration.Package?.Name ?? "",
            PackagePrice = registration.Package?.PriceMonthly ?? 0,
            Note = registration.Note,
            Status = registration.Status,
            AssignedStaffId = registration.AssignedStaffId,
            AssignedStaffName = registration.AssignedStaff?.Name,
            CreatedAt = registration.CreatedAt,
            UpdatedAt = registration.UpdatedAt
        };
    }

    public async Task<List<RegistrationResponseDto>> GetAllRegistrations()
    {
        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return registrations.Select(r => new RegistrationResponseDto
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
        }).ToList();
    }

    public async Task<List<RegistrationResponseDto>> GetUserRegistrations(int userId)
    {
        var registrations = await _context.Registrations
            .Include(r => r.Package)
            .Include(r => r.AssignedStaff)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return registrations.Select(r => new RegistrationResponseDto
        {
            Id = r.Id,
            UserId = r.UserId,
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
        }).ToList();
    }

    public async Task<RegistrationResponseDto?> UpdateStatus(int id, UpdateRegistrationStatusDto dto)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null) return null;

        registration.Status = dto.Status;
        registration.AssignedStaffId = dto.AssignedStaffId;
        registration.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetRegistrationById(id);
    }

    public async Task<bool> DeleteRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);
        if (registration == null) return false;

        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync();
        return true;
    }
}