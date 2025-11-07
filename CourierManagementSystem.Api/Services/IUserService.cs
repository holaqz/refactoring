using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;

namespace CourierManagementSystem.Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync(UserRole? role);
    Task<UserDto> CreateUserAsync(UserRequest request);
    Task<UserDto> UpdateUserAsync(long id, UserUpdateRequest request);
    Task DeleteUserAsync(long id);
}
