using BCrypt.Net;
using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(UserRole? role)
    {
        var users = role.HasValue
            ? await _userRepository.GetByRoleAsync(role.Value)
            : await _userRepository.GetAllAsync();

        return users.Select(UserDto.From).ToList();
    }

    public async Task<UserDto> CreateUserAsync(UserRequest request)
    {
        if (await _userRepository.ExistsByLoginAsync(request.Login))
        {
            throw new ValidationException($"Пользователь с логином '{request.Login}' уже существует");
        }

        var user = new User
        {
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserDto.From(user);
    }

    public async Task<UserDto> UpdateUserAsync(long id, UserUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        if (!string.IsNullOrEmpty(request.Login) && request.Login != user.Login)
        {
            if (await _userRepository.ExistsByLoginAsync(request.Login))
            {
                throw new ValidationException($"Пользователь с логином '{request.Login}' уже существует");
            }
            user.Login = request.Login;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            user.Name = request.Name;
        }

        if (request.Role.HasValue)
        {
            user.Role = request.Role.Value;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserDto.From(user);
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
