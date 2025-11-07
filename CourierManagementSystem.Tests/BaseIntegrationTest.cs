using CourierManagementSystem.Api.Configuration;
using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests;

public class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext DbContext;

    // Test users
    protected User AdminUser { get; private set; } = null!;
    protected User ManagerUser { get; private set; } = null!;
    protected User CourierUser { get; private set; } = null!;

    // Test tokens
    protected string AdminToken { get; private set; } = null!;
    protected string ManagerToken { get; private set; } = null!;
    protected string CourierToken { get; private set; } = null!;

    public BaseIntegrationTest(WebApplicationFactory<Program> factory)
    {
        var dbName = $"TestDb_{Guid.NewGuid()}";

        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing with shared name
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Mock OpenStreetMapService for tests
                var osmDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IOpenStreetMapService));
                if (osmDescriptor != null)
                {
                    services.Remove(osmDescriptor);
                }
                services.AddScoped<IOpenStreetMapService, MockOpenStreetMapService>();
            });
        });

        Client = Factory.CreateClient();

        // Get DbContext from the Server's Services (after the host is built)
        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        InitializeTestData().Wait();
    }

    private async Task InitializeTestData()
    {
        // Ensure admin user exists (seeded in ApplicationDbContext)
        AdminUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == "admin");
        if (AdminUser == null)
        {
            AdminUser = new User
            {
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Name = "Системный администратор",
                Role = UserRole.admin,
                CreatedAt = DateTime.UtcNow
            };
            DbContext.Users.Add(AdminUser);
        }

        // Ensure manager user exists
        ManagerUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == "manager");
        if (ManagerUser == null)
        {
            ManagerUser = new User
            {
                Login = "manager",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Name = "Менеджер",
                Role = UserRole.manager,
                CreatedAt = DateTime.UtcNow
            };
            DbContext.Users.Add(ManagerUser);
        }

        // Ensure courier user exists
        CourierUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == "courier");
        if (CourierUser == null)
        {
            CourierUser = new User
            {
                Login = "courier",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Name = "Курьер",
                Role = UserRole.courier,
                CreatedAt = DateTime.UtcNow
            };
            DbContext.Users.Add(CourierUser);
        }

        await DbContext.SaveChangesAsync();

        // Generate tokens
        var scope = Factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

        AdminToken = jwtService.GenerateToken(AdminUser);
        ManagerToken = jwtService.GenerateToken(ManagerUser);
        CourierToken = jwtService.GenerateToken(CourierUser);
    }

    private HttpRequestMessage CreateRequestWithAuth(HttpMethod method, string url, string token)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    protected async Task<T?> PostJsonAsync<T>(string url, object data, string? token = null)
    {
        HttpResponseMessage response;
        if (token != null)
        {
            var request = CreateRequestWithAuth(HttpMethod.Post, url, token);
            request.Content = JsonContent.Create(data);
            response = await Client.SendAsync(request);
        }
        else
        {
            response = await Client.PostAsJsonAsync(url, data);
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<T?> GetAsync<T>(string url, string? token = null)
    {
        HttpResponseMessage response;
        if (token != null)
        {
            var request = CreateRequestWithAuth(HttpMethod.Get, url, token);
            response = await Client.SendAsync(request);
        }
        else
        {
            response = await Client.GetAsync(url);
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<HttpResponseMessage> GetWithAuthAsync(string url, string token)
    {
        var request = CreateRequestWithAuth(HttpMethod.Get, url, token);
        return await Client.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> PostWithAuthAsync(string url, object data, string token)
    {
        var request = CreateRequestWithAuth(HttpMethod.Post, url, token);
        request.Content = JsonContent.Create(data);
        return await Client.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> PutWithAuthAsync(string url, object data, string token)
    {
        var request = CreateRequestWithAuth(HttpMethod.Put, url, token);
        request.Content = JsonContent.Create(data);
        return await Client.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> DeleteWithAuthAsync(string url, string token)
    {
        var request = CreateRequestWithAuth(HttpMethod.Delete, url, token);
        return await Client.SendAsync(request);
    }

    // Helper methods to create test data
    protected async Task<Vehicle> CreateVehicleAsync(
        string brand = "Ford Transit",
        string licensePlate = "А123БВ",
        decimal maxWeight = 1000m,
        decimal maxVolume = 15m)
    {
        var vehicle = new Vehicle
        {
            Brand = brand,
            LicensePlate = licensePlate,
            MaxWeight = maxWeight,
            MaxVolume = maxVolume
        };

        DbContext.Vehicles.Add(vehicle);
        await DbContext.SaveChangesAsync();
        return vehicle;
    }

    protected async Task<Product> CreateProductAsync(
        string name = "Тестовый товар",
        decimal weight = 1.5m,
        decimal length = 10m,
        decimal width = 10m,
        decimal height = 10m)
    {
        var product = new Product
        {
            Name = name,
            Weight = weight,
            Length = length,
            Width = width,
            Height = height
        };

        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();
        return product;
    }

    protected async Task<Delivery> CreateDeliveryAsync(long? courierId = null, long? vehicleId = null)
    {
        var actualCourierId = courierId ?? CourierUser.Id;
        Vehicle? vehicle = null;

        if (vehicleId.HasValue)
        {
            vehicle = await DbContext.Vehicles.FindAsync(vehicleId.Value);
        }
        else
        {
            vehicle = await CreateVehicleAsync();
            vehicleId = vehicle.Id;
        }

        var delivery = new Delivery
        {
            CourierId = actualCourierId,
            VehicleId = vehicleId,
            Vehicle = vehicle,
            CreatedById = ManagerUser.Id,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            TimeStart = new TimeOnly(9, 0),
            TimeEnd = new TimeOnly(18, 0),
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Deliveries.Add(delivery);
        await DbContext.SaveChangesAsync();

        var product = await CreateProductAsync();

        var deliveryPoint = new DeliveryPoint
        {
            DeliveryId = delivery.Id,
            Delivery = delivery,
            Sequence = 1,
            Latitude = 55.7558m,
            Longitude = 37.6176m
        };

        delivery.DeliveryPoints.Add(deliveryPoint);
        DbContext.DeliveryPoints.Add(deliveryPoint);
        await DbContext.SaveChangesAsync();

        var deliveryPointProduct = new DeliveryPointProduct
        {
            DeliveryPointId = deliveryPoint.Id,
            DeliveryPoint = deliveryPoint,
            ProductId = product.Id,
            Product = product,
            Quantity = 2
        };

        deliveryPoint.DeliveryPointProducts.Add(deliveryPointProduct);
        DbContext.DeliveryPointProducts.Add(deliveryPointProduct);
        await DbContext.SaveChangesAsync();

        return delivery;
    }
}

// Mock OpenStreetMapService for testing
public class MockOpenStreetMapService : IOpenStreetMapService
{
    private const decimal CoordinateTolerance = 0.0001m;

    public Task<decimal> CalculateDistanceAsync(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        if (IsRoute(lat1, lon1, lat2, lon2, 55.7558m, 37.6176m, 59.9311m, 30.3609m))
        {
            return Task.FromResult(635.0m);
        }

        if (IsRoute(lat1, lon1, lat2, lon2, 55.7558m, 37.6176m, 55.7600m, 37.6200m))
        {
            return Task.FromResult(2.5m);
        }

        if (IsRoute(lat1, lon1, lat2, lon2, 55.7600m, 37.6200m, 55.7700m, 37.6300m))
        {
            return Task.FromResult(0.1m);
        }

        if (AreCoordinatesEqual(lat1, lat2) && AreCoordinatesEqual(lon1, lon2))
        {
            return Task.FromResult(0.1m);
        }

        // Default fallback distance for scenarios not explicitly mapped
        return Task.FromResult(10.5m);
    }

    private static bool IsRoute(decimal lat1, decimal lon1, decimal lat2, decimal lon2,
        decimal expectedLat1, decimal expectedLon1, decimal expectedLat2, decimal expectedLon2)
    {
        return (AreCoordinatesEqual(lat1, expectedLat1) &&
                AreCoordinatesEqual(lon1, expectedLon1) &&
                AreCoordinatesEqual(lat2, expectedLat2) &&
                AreCoordinatesEqual(lon2, expectedLon2))
               ||
               (AreCoordinatesEqual(lat1, expectedLat2) &&
                AreCoordinatesEqual(lon1, expectedLon2) &&
                AreCoordinatesEqual(lat2, expectedLat1) &&
                AreCoordinatesEqual(lon2, expectedLon1));
    }

    private static bool AreCoordinatesEqual(decimal a, decimal b) =>
        Math.Abs(a - b) < CoordinateTolerance;
}
