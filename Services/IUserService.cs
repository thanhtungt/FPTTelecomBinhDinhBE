using FPTTelecomBE.DTOs.User;

namespace FPTTelecomBE.Services;

public interface IUserService
{
    Task<UserDto?> GetUserById(int id);
    Task<List<UserDto>> GetAllUsers(string? role = null);
    Task<UserDto> CreateUser(CreateUserDto dto);
    Task<UserDto?> UpdateUser(int id, UpdateUserDto dto);
    Task<UserDto?> UpdateUserRole(int id, string role, int adminId);
    Task<bool> DeleteUser(int id, int adminId);
    Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
    Task<int> GetTotalUsersCount();
    Task<Dictionary<string, int>> GetUserStatsByRole();
}