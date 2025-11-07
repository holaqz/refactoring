using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class CourierControllerTest : BaseIntegrationTest
{
    public CourierControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetCourierDeliveries_WithCourierToken_ShouldReturnOwnDeliveries()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync("/courier/deliveries", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.NotEmpty(deliveries);
    }

    [Fact]
    public async Task GetCourierDeliveries_WithManagerToken_ShouldReturnForbidden()
    {
        // Act
        var response = await GetWithAuthAsync("/courier/deliveries", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveries_WithAdminToken_ShouldReturnForbidden()
    {
        // Act
        var response = await GetWithAuthAsync("/courier/deliveries", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveries_WithDateFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries?date={delivery.DeliveryDate:yyyy-MM-dd}", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.All(deliveries, d => Assert.Equal(delivery.DeliveryDate, d.DeliveryDate));
    }

    [Fact]
    public async Task GetCourierDeliveries_WithNonMatchingDateFilter_ShouldReturnEmpty()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries?date={futureDate:yyyy-MM-dd}", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.Empty(deliveries);
    }

    [Fact]
    public async Task GetCourierDeliveries_WithStatusFilter_ShouldReturnPlannedDeliveries()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync("/courier/deliveries?status=planned", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.All(deliveries, d => Assert.Equal(DeliveryStatus.planned, d.Status));
    }

    [Fact]
    public async Task GetCourierDeliveries_WithDateRange_ShouldReturnResults()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        var dateFrom = delivery.DeliveryDate.AddDays(-1);
        var dateTo = delivery.DeliveryDate.AddDays(1);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.NotEmpty(deliveries);
    }

    [Fact]
    public async Task GetCourierDeliveries_ShouldNotReturnOtherCourierDeliveries()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var anotherCourier = new User
        {
            Login = "courier-other",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Name = "Другой Курьер",
            Role = UserRole.courier,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.Users.Add(anotherCourier);
        await DbContext.SaveChangesAsync();

        await CreateDeliveryAsync(anotherCourier.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync("/courier/deliveries", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<CourierDeliveryResponse>>();
        Assert.NotNull(deliveries);
        Assert.Empty(deliveries);
    }

    [Fact]
    public async Task GetCourierDeliveryById_WithValidId_ShouldReturnDeliveryDetails()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries/{delivery.Id}", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CourierDeliveryResponse>();
        Assert.NotNull(result);
        Assert.Equal(delivery.Id, result.Id);
        Assert.Equal(DeliveryStatus.planned, result.Status);
        Assert.NotNull(result.Vehicle);
        Assert.Equal("Ford Transit", result.Vehicle?.Brand);
        Assert.NotNull(result.DeliveryPoints);
        Assert.NotEmpty(result.DeliveryPoints);
    }

    [Fact]
    public async Task GetCourierDeliveryById_WithOtherCourier_ShouldReturnForbidden()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();

        // Create another courier
        var anotherCourier = new User
        {
            Login = "courier2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Name = "Другой Курьер",
            Role = UserRole.courier,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.Users.Add(anotherCourier);
        await DbContext.SaveChangesAsync();

        // Create delivery for another courier
        var delivery = await CreateDeliveryAsync(anotherCourier.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries/{delivery.Id}", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveryById_WithAdminToken_ShouldReturnForbidden()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries/{delivery.Id}", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveryById_WithManagerToken_ShouldReturnForbidden()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/courier/deliveries/{delivery.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveryById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await GetWithAuthAsync("/courier/deliveries/99999", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCourierDeliveries_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/courier/deliveries");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
