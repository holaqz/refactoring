using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class VehicleControllerTest : BaseIntegrationTest
{
    public VehicleControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllVehicles_WithManagerToken_ShouldReturnVehicles()
    {
        // Arrange
        await CreateVehicleAsync();

        // Act
        var response = await GetWithAuthAsync("/vehicles", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleDto>>();
        Assert.NotNull(vehicles);
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    public async Task GetAllVehicles_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_AsAdmin_ShouldReturnCreatedVehicle()
    {
        // Arrange
        var request = new VehicleRequest
        {
            Brand = "Ford Transit",
            LicensePlate = "А321БВ",
            MaxWeight = 1000m,
            MaxVolume = 15m
        };

        // Act
        var response = await PostWithAuthAsync("/vehicles", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>();
        Assert.NotNull(vehicle);
        Assert.Equal("Ford Transit", vehicle.Brand);
        Assert.Equal("А321БВ", vehicle.LicensePlate);
    }

    [Fact]
    public async Task CreateVehicle_AsManager_ShouldReturnForbidden()
    {
        // Arrange
        var request = new VehicleRequest
        {
            Brand = "Ford Transit",
            LicensePlate = "А321БВ",
            MaxWeight = 1000m,
            MaxVolume = 15m
        };

        // Act
        var response = await PostWithAuthAsync("/vehicles", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_WithDuplicateLicensePlate_ShouldReturnBadRequest()
    {
        // Arrange
        await CreateVehicleAsync("Ford Transit", "А777АА");

        var request = new VehicleRequest
        {
            Brand = "Ford Transit",
            LicensePlate = "А777АА",
            MaxWeight = 1000m,
            MaxVolume = 15m
        };

        // Act
        var response = await PostWithAuthAsync("/vehicles", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVehicle_WithValidData_ShouldReturnUpdatedVehicle()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync("Old Brand", "UPD123");

        var request = new VehicleRequest
        {
            Brand = "New Brand",
            LicensePlate = "UPD123",
            MaxWeight = 3000m,
            MaxVolume = 30m
        };

        // Act
        var response = await PutWithAuthAsync($"/vehicles/{vehicle.Id}", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<VehicleDto>();
        Assert.NotNull(updated);
        Assert.Equal("New Brand", updated.Brand);
        Assert.Equal(3000m, updated.MaxWeight);
    }

    [Fact]
    public async Task UpdateVehicle_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new VehicleRequest
        {
            Brand = "Test",
            LicensePlate = "TEST123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        // Act
        var response = await PutWithAuthAsync("/vehicles/99999", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync("To Delete", "DEL123");

        // Act
        var response = await DeleteWithAuthAsync($"/vehicles/{vehicle.Id}", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteWithAuthAsync("/vehicles/99999", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_AsManager_ShouldReturnForbidden()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();

        // Act
        var response = await DeleteWithAuthAsync($"/vehicles/{vehicle.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
