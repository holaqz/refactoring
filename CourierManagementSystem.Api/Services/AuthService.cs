using BCrypt.Net;
using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByLoginAsync(request.Login);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Неверный логин или пароль");
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpiresIn = 3600,
            User = UserDto.From(user)
        };
    }

    public async Task<UserDto?> GetCurrentUserAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? UserDto.From(user) : null;
    }
}
