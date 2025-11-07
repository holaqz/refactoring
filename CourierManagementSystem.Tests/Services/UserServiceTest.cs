using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithoutRoleFilter_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Login = "admin", Name = "Admin", Role = UserRole.admin, PasswordHash = "hash" },
            new User { Id = 2, Login = "manager", Name = "Manager", Role = UserRole.manager, PasswordHash = "hash" }
        };

        _userRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithRoleFilter_ReturnsFilteredUsers()
    {
        // Arrange
        var adminUsers = new List<User>
        {
            new User { Id = 1, Login = "admin", Name = "Admin", Role = UserRole.admin, PasswordHash = "hash" }
        };

        _userRepositoryMock.Setup(r => r.GetByRoleAsync(UserRole.admin))
            .ReturnsAsync(adminUsers);

        // Act
        var result = await _userService.GetAllUsersAsync(UserRole.admin);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(UserRole.admin, result[0].Role);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ReturnsCreatedUser()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.ExistsByLoginAsync("newuser"))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _userRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new UserRequest
        {
            Login = "newuser",
            Password = "password123",
            Name = "New User",
            Role = UserRole.courier
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Login);
        Assert.Equal("New User", result.Name);
        Assert.Equal(UserRole.courier, result.Role);
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateLogin_ThrowsValidationException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.ExistsByLoginAsync("duplicate"))
            .ReturnsAsync(true);

        var request = new UserRequest
        {
            Login = "duplicate",
            Password = "password123",
            Name = "Duplicate User",
            Role = UserRole.courier
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUserAsync(request));
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ReturnsUpdatedUser()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Login = "user",
            PasswordHash = "hash",
            Name = "Old Name",
            Role = UserRole.courier
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        _userRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new UserUpdateRequest
        {
            Name = "New Name"
        };

        // Act
        var result = await _userService.UpdateUserAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var request = new UserUpdateRequest
        {
            Name = "New Name"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(999, request));
    }

    [Fact]
    public async Task UpdateUserAsync_WithDuplicateLogin_ThrowsValidationException()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Login = "user",
            PasswordHash = "hash",
            Name = "User",
            Role = UserRole.courier
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(r => r.ExistsByLoginAsync("duplicate"))
            .ReturnsAsync(true);

        var request = new UserUpdateRequest
        {
            Login = "duplicate"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUserAsync(1, request));
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_Succeeds()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Login = "user",
            PasswordHash = "hash",
            Name = "User",
            Role = UserRole.courier
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(r => r.DeleteAsync(user))
            .Returns(Task.CompletedTask);

        _userRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _userService.DeleteUserAsync(1);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(999));
    }
}
