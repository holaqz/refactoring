using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class CourierServiceTest
{
    private readonly Mock<IDeliveryRepository> _deliveryRepositoryMock;
    private readonly CourierService _courierService;

    public CourierServiceTest()
    {
        _deliveryRepositoryMock = new Mock<IDeliveryRepository>();
        _courierService = new CourierService(_deliveryRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCourierDeliveriesAsync_WithNoFilters_ReturnsAllCourierDeliveries()
    {
        // Arrange
        var deliveries = new List<Delivery>
        {
            CreateSampleDelivery(1, 1),
            CreateSampleDelivery(2, 1)
        };

        _deliveryRepositoryMock.Setup(r => r.GetByCourierWithDetailsAsync(1))
            .ReturnsAsync(deliveries);

        // Act
        var result = await _courierService.GetCourierDeliveriesAsync(1, null, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCourierDeliveriesAsync_WithDateFilter_ReturnsFilteredDeliveries()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var deliveries = new List<Delivery>
        {
            CreateSampleDelivery(1, 1, date)
        };

        _deliveryRepositoryMock.Setup(r => r.GetByDeliveryDateAndCourierIdWithDetailsAsync(date, 1))
            .ReturnsAsync(deliveries);

        // Act
        var result = await _courierService.GetCourierDeliveriesAsync(1, date, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, d => Assert.Equal(date, d.DeliveryDate));
    }

    [Fact]
    public async Task GetCourierDeliveriesAsync_WithDateRange_ReturnsDeliveriesInRange()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var deliveries = new List<Delivery>
        {
            CreateSampleDelivery(1, 1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)))
        };

        _deliveryRepositoryMock.Setup(r => r.GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(1, dateFrom, dateTo))
            .ReturnsAsync(deliveries);

        // Act
        var result = await _courierService.GetCourierDeliveriesAsync(1, null, null, dateFrom, dateTo);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetCourierDeliveriesAsync_WithStatusFilter_ReturnsFilteredDeliveries()
    {
        // Arrange
        var deliveries = new List<Delivery>
        {
            CreateSampleDelivery(1, 1)
        };

        _deliveryRepositoryMock.Setup(r => r.GetByCourierWithDetailsAsync(1))
            .ReturnsAsync(deliveries);

        // Act
        var result = await _courierService.GetCourierDeliveriesAsync(1, null, DeliveryStatus.planned, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, d => Assert.Equal(DeliveryStatus.planned, d.Status));
    }

    [Fact]
    public async Task GetCourierDeliveryByIdAsync_WithValidIdAndCourier_ReturnsDelivery()
    {
        // Arrange
        var delivery = CreateSampleDelivery(1, 1);

        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(delivery);

        // Act
        var result = await _courierService.GetCourierDeliveryByIdAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetCourierDeliveryByIdAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Delivery?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _courierService.GetCourierDeliveryByIdAsync(1, 999));
    }

    [Fact]
    public async Task GetCourierDeliveryByIdAsync_WithWrongCourier_ThrowsForbiddenException()
    {
        // Arrange
        var delivery = CreateSampleDelivery(1, 2); // Delivery belongs to courier 2

        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(delivery);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _courierService.GetCourierDeliveryByIdAsync(1, 1)); // Courier 1 trying to access
    }

    private Delivery CreateSampleDelivery(long id, long courierId, DateOnly? date = null)
    {
        return new Delivery
        {
            Id = id,
            CourierId = courierId,
            VehicleId = 1,
            CreatedById = 1,
            DeliveryDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            TimeStart = new TimeOnly(9, 0),
            TimeEnd = new TimeOnly(17, 0),
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Courier = new User { Id = courierId, Login = "courier", Name = "Courier", Role = UserRole.courier, PasswordHash = "hash" },
            Vehicle = new Vehicle { Id = 1, Brand = "Mercedes", LicensePlate = "ABC123", MaxWeight = 1000m, MaxVolume = 10m },
            CreatedBy = new User { Id = 2, Login = "manager", Name = "Manager", Role = UserRole.manager, PasswordHash = "hash" },
            DeliveryPoints = new List<DeliveryPoint>()
        };
    }
}
