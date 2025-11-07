using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class AuthServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _authService = new AuthService(_userRepositoryMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Name = "Test User",
            Role = UserRole.manager
        };

        _userRepositoryMock.Setup(r => r.GetByLoginAsync("testuser"))
            .ReturnsAsync(user);

        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("fake-jwt-token");

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "password123"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result.Token);
        Assert.NotNull(result.User);
        Assert.Equal("testuser", result.User.Login);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Name = "Test User",
            Role = UserRole.manager
        };

        _userRepositoryMock.Setup(r => r.GetByLoginAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "wrongpassword"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByLoginAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest
        {
            Login = "nonexistent",
            Password = "password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithValidUserId_ReturnsUserDto()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            PasswordHash = "hash",
            Name = "Test User",
            Role = UserRole.manager
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetCurrentUserAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Login);
        Assert.Equal("Test User", result.Name);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithNonExistentUserId_ReturnsNull()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.GetCurrentUserAsync(999);

        // Assert
        Assert.Null(result);
    }
}
