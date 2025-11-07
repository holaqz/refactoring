using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class RouteServiceTest
{
    private readonly Mock<IOpenStreetMapService> _osmServiceMock;
    private readonly RouteService _routeService;

    public RouteServiceTest()
    {
        _osmServiceMock = new Mock<IOpenStreetMapService>();
        _routeService = new RouteService(_osmServiceMock.Object);
    }

    [Fact]
    public async Task CalculateRouteAsync_WithMultiplePoints_ReturnsCalculation()
    {
        // Arrange
        _osmServiceMock.Setup(s => s.CalculateDistanceAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(10.5m);

        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest { Latitude = 55.7558m, Longitude = 37.6173m, Products = new List<DeliveryProductRequest>() },
                new DeliveryPointRequest { Latitude = 55.7540m, Longitude = 37.6200m, Products = new List<DeliveryProductRequest>() }
            }
        };

        // Act
        var result = await _routeService.CalculateRouteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10.5m, result.DistanceKm);
        Assert.True(result.DurationMinutes > 0);
        Assert.NotNull(result.SuggestedTime);
    }

    [Fact]
    public async Task CalculateRouteAsync_WithSinglePoint_ReturnsZeroDistance()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest { Latitude = 55.7558m, Longitude = 37.6173m, Products = new List<DeliveryProductRequest>() }
            }
        };

        // Act
        var result = await _routeService.CalculateRouteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.DistanceKm);
        Assert.Equal(0, result.DurationMinutes);
    }

    [Fact]
    public async Task CalculateRouteAsync_WithNoPoints_ReturnsZeroDistance()
    {
        // Arrange
        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>()
        };

        // Act
        var result = await _routeService.CalculateRouteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.DistanceKm);
        Assert.Equal(0, result.DurationMinutes);
    }

    [Fact]
    public async Task CalculateRouteAsync_WithThreePoints_CalculatesTotalDistance()
    {
        // Arrange
        _osmServiceMock.Setup(s => s.CalculateDistanceAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(10.5m);

        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest { Latitude = 55.7558m, Longitude = 37.6173m, Products = new List<DeliveryProductRequest>() },
                new DeliveryPointRequest { Latitude = 55.7540m, Longitude = 37.6200m, Products = new List<DeliveryProductRequest>() },
                new DeliveryPointRequest { Latitude = 55.7520m, Longitude = 37.6230m, Products = new List<DeliveryProductRequest>() }
            }
        };

        // Act
        var result = await _routeService.CalculateRouteAsync(request);

        // Assert
        Assert.NotNull(result);
        // 2 segments * 10.5 km = 21 km
        Assert.Equal(21m, result.DistanceKm);
        // Duration includes time for 3 delivery points (15 minutes total for stops)
        Assert.True(result.DurationMinutes >= 15);

        // Verify CalculateDistanceAsync was called twice (for 2 segments)
        _osmServiceMock.Verify(s => s.CalculateDistanceAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task CalculateRouteAsync_IncludesTimeForLoadingUnloading()
    {
        // Arrange
        _osmServiceMock.Setup(s => s.CalculateDistanceAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(40m); // 40 km distance, 1 hour travel time at 40 km/h

        var request = new RouteCalculationRequest
        {
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest { Latitude = 55.7558m, Longitude = 37.6173m, Products = new List<DeliveryProductRequest>() },
                new DeliveryPointRequest { Latitude = 55.7540m, Longitude = 37.6200m, Products = new List<DeliveryProductRequest>() }
            }
        };

        // Act
        var result = await _routeService.CalculateRouteAsync(request);

        // Assert
        // 1 hour travel + 10 minutes for 2 delivery points = 70 minutes
        Assert.Equal(70, result.DurationMinutes);
    }
}
