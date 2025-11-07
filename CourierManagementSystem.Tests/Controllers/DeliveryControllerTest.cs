using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class DeliveryControllerTest : BaseIntegrationTest
{
    public DeliveryControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllDeliveries_WithManagerToken_ReturnsDeliveries()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync("/deliveries", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<DeliveryDto>>();
        Assert.NotNull(deliveries);
        Assert.NotEmpty(deliveries);
    }

    [Fact]
    public async Task GetAllDeliveries_WithCourierToken_ReturnsForbidden()
    {
        // Act
        var response = await GetWithAuthAsync("/deliveries", CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDeliveries_WithDateFilter_ReturnsFilteredDeliveries()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/deliveries?date={delivery.DeliveryDate:yyyy-MM-dd}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deliveries = await response.Content.ReadFromJsonAsync<List<DeliveryDto>>();
        Assert.NotNull(deliveries);
        Assert.All(deliveries, d => Assert.Equal(delivery.DeliveryDate, d.DeliveryDate));
    }

    [Fact]
    public async Task GetDeliveryById_WithValidId_ReturnsDelivery()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await GetWithAuthAsync($"/deliveries/{delivery.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DeliveryDto>();
        Assert.NotNull(result);
        Assert.Equal(delivery.Id, result.Id);
    }

    [Fact]
    public async Task GetDeliveryById_WithNonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await GetWithAuthAsync("/deliveries/99999", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateDelivery_WithValidData_ReturnsCreatedDelivery()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var product = await CreateProductAsync();

        var request = new DeliveryRequest
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            TimeStart = new TimeOnly(9, 0),
            TimeEnd = new TimeOnly(18, 0),
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>
                    {
                        new DeliveryProductRequest
                        {
                            ProductId = product.Id,
                            Quantity = 3
                        }
                    }
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var delivery = await response.Content.ReadFromJsonAsync<DeliveryDto>();
        Assert.NotNull(delivery);
        Assert.Equal(CourierUser.Id, delivery.Courier?.Id);
        Assert.Equal(vehicle.Id, delivery.Vehicle?.Id);
        Assert.Single(delivery.DeliveryPoints);
    }

    [Fact]
    public async Task CreateDelivery_WithNonExistentCourier_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeliveryRequest
        {
            CourierId = 99999,
            VehicleId = null,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateDelivery_WithTimeConflict_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var deliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

        // Create first delivery
        var firstDelivery = new Delivery
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            CreatedById = ManagerUser.Id,
            DeliveryDate = deliveryDate,
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(14, 0),
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Deliveries.Add(firstDelivery);
        await DbContext.SaveChangesAsync();

        // Try to create overlapping delivery
        var request = new DeliveryRequest
        {
            CourierId = CourierUser.Id,
            VehicleId = null,
            DeliveryDate = deliveryDate,
            TimeStart = new TimeOnly(12, 0), // Overlaps with first delivery
            TimeEnd = new TimeOnly(16, 0),
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateDelivery_ExceedingVehicleCapacity_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        vehicle.MaxWeight = 10m; // Small capacity
        await DbContext.SaveChangesAsync();

        var product = await CreateProductAsync("Heavy Product");
        product.Weight = 100m; // Heavy product
        await DbContext.SaveChangesAsync();

        var request = new DeliveryRequest
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>
                    {
                        new DeliveryProductRequest
                        {
                            ProductId = product.Id,
                            Quantity = 1
                        }
                    }
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDelivery_WithValidData_ReturnsUpdatedDelivery()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        var product = await CreateProductAsync();

        var request = new DeliveryRequest
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            DeliveryDate = delivery.DeliveryDate,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(17, 0),
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>
                    {
                        new DeliveryProductRequest
                        {
                            ProductId = product.Id,
                            Quantity = 2
                        }
                    }
                }
            }
        };

        // Act
        var response = await PutWithAuthAsync($"/deliveries/{delivery.Id}", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<DeliveryDto>();
        Assert.NotNull(updated);
        Assert.Equal(new TimeOnly(11, 0), updated.TimeStart);
        Assert.Equal(new TimeOnly(17, 0), updated.TimeEnd);
    }

    [Fact]
    public async Task UpdateDelivery_LessThan3DaysBeforeDelivery_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = new Delivery
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            CreatedById = ManagerUser.Id,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), // Only 2 days away
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Deliveries.Add(delivery);
        await DbContext.SaveChangesAsync();

        var request = new DeliveryRequest
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            DeliveryDate = delivery.DeliveryDate,
            TimeStart = new TimeOnly(11, 0),
            TimeEnd = new TimeOnly(17, 0),
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var response = await PutWithAuthAsync($"/deliveries/{delivery.Id}", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDelivery_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = await CreateDeliveryAsync(CourierUser.Id, vehicle.Id);

        // Act
        var response = await DeleteWithAuthAsync($"/deliveries/{delivery.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDelivery_InThePast_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = await CreateVehicleAsync();
        var delivery = new Delivery
        {
            CourierId = CourierUser.Id,
            VehicleId = vehicle.Id,
            CreatedById = ManagerUser.Id,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), // In the past
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Status = DeliveryStatus.completed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Deliveries.Add(delivery);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteWithAuthAsync($"/deliveries/{delivery.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GenerateDeliveries_WithValidData_ReturnsGeneratedDeliveries()
    {
        // Arrange
        var product = await CreateProductAsync();

        var request = new GenerateDeliveriesRequest
        {
            DeliveryData = new Dictionary<DateOnly, List<RouteWithProducts>>
            {
                {
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    new List<RouteWithProducts>
                    {
                        new RouteWithProducts
                        {
                            Route = new List<DeliveryPointRequest>
                            {
                                new DeliveryPointRequest
                                {
                                    Latitude = 55.7558m,
                                    Longitude = 37.6173m,
                                    Products = new List<DeliveryProductRequest>()
                                }
                            },
                            Products = new List<DeliveryProductRequest>
                            {
                                new DeliveryProductRequest
                                {
                                    ProductId = product.Id,
                                    Quantity = 3
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries/generate", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GenerateDeliveriesResponse>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenerateDeliveries_WithCourierToken_ReturnsForbidden()
    {
        // Arrange
        var request = new GenerateDeliveriesRequest
        {
            DeliveryData = new Dictionary<DateOnly, List<RouteWithProducts>>()
        };

        // Act
        var response = await PostWithAuthAsync("/deliveries/generate", request, CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
