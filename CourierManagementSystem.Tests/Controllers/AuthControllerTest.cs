using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class AuthControllerTest : BaseIntegrationTest
{
    public AuthControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokenAndUserInfo()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "admin",
            Password = "admin123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotNull(loginResponse.User);
        Assert.Equal(AdminUser.Id, loginResponse.User.Id);
        Assert.Equal("admin", loginResponse.User.Login);
        Assert.Equal("Системный администратор", loginResponse.User.Name);
        Assert.Equal(UserRole.admin, loginResponse.User.Role);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "admin",
            Password = "wrongpassword"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownLogin_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "nonexistent",
            Password = "password"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyLogin_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = string.Empty,
            Password = "admin123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "admin",
            Password = string.Empty
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithManagerCredentials_ShouldReturnManagerToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "manager",
            Password = "password"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotNull(loginResponse.User);
        Assert.Equal(ManagerUser.Id, loginResponse.User.Id);
        Assert.Equal("manager", loginResponse.User.Login);
        Assert.Equal("Менеджер", loginResponse.User.Name);
        Assert.Equal(UserRole.manager, loginResponse.User.Role);
    }

    [Fact]
    public async Task Login_WithCourierCredentials_ShouldReturnCourierToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "courier",
            Password = "password"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotNull(loginResponse.User);
        Assert.Equal(CourierUser.Id, loginResponse.User.Id);
        Assert.Equal("courier", loginResponse.User.Login);
        Assert.Equal("Курьер", loginResponse.User.Name);
        Assert.Equal(UserRole.courier, loginResponse.User.Role);
    }

    [Fact]
    public async Task Debug_WithValidToken_ShouldReturnUserInfo()
    {
        // Act
        var response = await GetWithAuthAsync("/auth/debug", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Debug_WithoutToken_ShouldReturnNotAuthenticatedResponse()
    {
        // Act
        var response = await Client.GetAsync("/auth/debug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
