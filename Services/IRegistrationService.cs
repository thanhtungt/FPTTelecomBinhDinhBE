using FPTTelecomBE.DTOs.Registration;

namespace FPTTelecomBE.Services;

public interface IRegistrationService
{
    Task<RegistrationResponseDto> CreateRegistration(CreateRegistrationDto dto);
    Task<RegistrationResponseDto?> GetRegistrationById(int id);
    Task<List<RegistrationResponseDto>> GetAllRegistrations();
    Task<List<RegistrationResponseDto>> GetUserRegistrations(int userId);
    Task<RegistrationResponseDto?> UpdateStatus(int id, UpdateRegistrationStatusDto dto);
    Task<bool> DeleteRegistration(int id);
}