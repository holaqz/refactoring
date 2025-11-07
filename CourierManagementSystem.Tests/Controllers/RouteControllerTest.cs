using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class RouteControllerTest : BaseIntegrationTest
{
    public RouteControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CalculateRoute_WithMultiplePoints_ReturnsCalculation()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>()
                },
                new DeliveryPointRequest
                {
                    Latitude = 55.7540m,
                    Longitude = 37.6200m,
                    Products = new List<DeliveryProductRequest>()
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/routes/calculate", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RouteCalculationResponse>();
        Assert.NotNull(result);
        Assert.True(result.DistanceKm > 0);
        Assert.True(result.DurationMinutes > 0);
        Assert.NotNull(result.SuggestedTime);
    }

    [Fact]
    public async Task CalculateRoute_WithSinglePoint_ReturnsZeroDistance()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>()
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/routes/calculate", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RouteCalculationResponse>();
        Assert.NotNull(result);
        Assert.Equal(0, result.DistanceKm);
        Assert.Equal(0, result.DurationMinutes);
    }

    [Fact]
    public async Task CalculateRoute_WithNoPoints_ReturnsZeroDistance()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var response = await PostWithAuthAsync("/routes/calculate", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RouteCalculationResponse>();
        Assert.NotNull(result);
        Assert.Equal(0, result.DistanceKm);
        Assert.Equal(0, result.DurationMinutes);
    }

    [Fact]
    public async Task CalculateRoute_WithCourierToken_Succeeds()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>()
                },
                new DeliveryPointRequest
                {
                    Latitude = 55.7540m,
                    Longitude = 37.6200m,
                    Products = new List<DeliveryProductRequest>()
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/routes/calculate", request, CourierToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CalculateRoute_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var response = await Client.PostAsJsonAsync("/routes/calculate", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CalculateRoute_WithThreePoints_CalculatesCorrectly()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.7558m,
                    Longitude = 37.6173m,
                    Products = new List<DeliveryProductRequest>()
                },
                new DeliveryPointRequest
                {
                    Latitude = 55.7540m,
                    Longitude = 37.6200m,
                    Products = new List<DeliveryProductRequest>()
                },
                new DeliveryPointRequest
                {
                    Latitude = 55.7520m,
                    Longitude = 37.6230m,
                    Products = new List<DeliveryProductRequest>()
                }
            }
        };

        // Act
        var response = await PostWithAuthAsync("/routes/calculate", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RouteCalculationResponse>();
        Assert.NotNull(result);
        // With 3 points, distance should be for 2 segments
        // Mock service returns 10.5 km per segment, so total should be 21 km
        Assert.Equal(21m, result.DistanceKm);
        // Duration should account for 3 points (15 minutes for stops + travel time)
        Assert.True(result.DurationMinutes >= 15);
    }
}
