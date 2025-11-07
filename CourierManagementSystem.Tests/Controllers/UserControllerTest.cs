using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class UserControllerTest : BaseIntegrationTest
{
    public UserControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllUsers_AsAdmin_ShouldReturnAllUsers()
    {
        // Act
        var response = await GetWithAuthAsync("/users", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.True(users.Count >= 3); // At least admin, manager, courier
    }

    [Fact]
    public async Task GetAllUsers_AsManager_ShouldReturnForbidden()
    {
        // Act
        var response = await GetWithAuthAsync("/users", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_WithRoleFilter_ShouldReturnFilteredUsers()
    {
        // Act
        var response = await GetWithAuthAsync("/users?role=admin", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.All(users, u => Assert.Equal(UserRole.admin, u.Role));
    }

    [Fact]
    public async Task CreateUser_AsAdmin_ShouldReturnCreatedUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Login = "newcourier",
            Password = "password123",
            Name = "Новый Курьер",
            Role = UserRole.courier
        };

        // Act
        var response = await PostWithAuthAsync("/users", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("newcourier", user.Login);
        Assert.Equal("Новый Курьер", user.Name);
        Assert.Equal(UserRole.courier, user.Role);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateLogin_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Login = "admin", // Duplicate
            Password = "password123",
            Name = "Другой Админ",
            Role = UserRole.courier
        };

        // Act
        var response = await PostWithAuthAsync("/users", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_AsAdmin_ShouldReturnUpdatedUser()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            Name = "Обновленный Менеджер"
        };

        // Act
        var response = await PutWithAuthAsync($"/users/{ManagerUser.Id}", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("Обновленный Менеджер", user.Name);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            Name = "Updated User"
        };

        // Act
        var response = await PutWithAuthAsync("/users/99999", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_WithDuplicateLogin_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            Login = "admin" // Try to change to existing login
        };

        // Act
        var response = await PutWithAuthAsync($"/users/{ManagerUser.Id}", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_AsAdmin_ShouldReturnNoContent()
    {
        // Arrange - Create a user to delete
        var newUser = new UserRequest
        {
            Login = "todelete",
            Password = "password123",
            Name = "Для Удаления",
            Role = UserRole.courier
        };
        var createResponse = await PostWithAuthAsync("/users", newUser, AdminToken);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Act
        var response = await DeleteWithAuthAsync($"/users/{createdUser!.Id}", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteWithAuthAsync("/users/99999", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
