using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class DeliveryServiceTest
{
    private readonly Mock<IDeliveryRepository> _deliveryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IDeliveryPointRepository> _deliveryPointRepositoryMock;
    private readonly Mock<IDeliveryPointProductRepository> _deliveryPointProductRepositoryMock;
    private readonly Mock<IOpenStreetMapService> _openStreetMapServiceMock;
    private readonly DeliveryService _deliveryService;

    public DeliveryServiceTest()
    {
        _deliveryRepositoryMock = new Mock<IDeliveryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _deliveryPointRepositoryMock = new Mock<IDeliveryPointRepository>();
        _deliveryPointProductRepositoryMock = new Mock<IDeliveryPointProductRepository>();
        _openStreetMapServiceMock = new Mock<IOpenStreetMapService>();

        _deliveryService = new DeliveryService(
            _deliveryRepositoryMock.Object,
            _userRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _productRepositoryMock.Object,
            _deliveryPointRepositoryMock.Object,
            _deliveryPointProductRepositoryMock.Object,
            _openStreetMapServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllDeliveriesAsync_WithNoFilters_ReturnsAllDeliveries()
    {
        // Arrange
        var deliveries = new List<Delivery>
        {
            CreateSampleDelivery(1),
            CreateSampleDelivery(2)
        };

        _deliveryRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(deliveries);

        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<long>()))
            .ReturnsAsync((long id) => deliveries.FirstOrDefault(d => d.Id == id));

        // Act
        var result = await _deliveryService.GetAllDeliveriesAsync(null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDeliveryByIdAsync_WithValidId_ReturnsDelivery()
    {
        // Arrange
        var delivery = CreateSampleDelivery(1);

        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(delivery);

        // Act
        var result = await _deliveryService.GetDeliveryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetDeliveryByIdAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _deliveryRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Delivery?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _deliveryService.GetDeliveryByIdAsync(999));
    }

    [Fact]
    public async Task CreateDeliveryAsync_WithExceededVehicleCapacity_ThrowsValidationException()
    {
        // Arrange
        var courier = new User { Id = 1, Login = "courier", Role = UserRole.courier, PasswordHash = "hash", Name = "Courier" };
        var vehicle = new Vehicle { Id = 5, Brand = "Test", LicensePlate = "TEST123", MaxWeight = 100m, MaxVolume = 5m };
        var product = new Product { Id = 10, Name = "Product", Weight = 60m, Length = 10m, Width = 10m, Height = 10m };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(courier);

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(vehicle);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(product);

        _deliveryRepositoryMock.Setup(r => r.GetByDateVehicleAndOverlappingTimeAsync(
                It.IsAny<DateOnly>(),
                5,
                It.IsAny<TimeOnly>(),
                It.IsAny<TimeOnly>(),
                null))
            .ReturnsAsync(new List<Delivery>());

        var request = new DeliveryRequest
        {
            CourierId = 1,
            VehicleId = 5,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Points = new List<DeliveryPointRequest>
            {
                new DeliveryPointRequest
                {
                    Latitude = 55.0m,
                    Longitude = 37.0m,
                    Products = new List<DeliveryProductRequest>
                    {
                        new DeliveryProductRequest
                        {
                            ProductId = 10,
                            Quantity = 2
                        }
                    }
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _deliveryService.CreateDeliveryAsync(request, 1));
    }

    [Fact]
    public async Task CreateDeliveryAsync_WithNonExistentCourier_ThrowsValidationException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var request = new DeliveryRequest
        {
            CourierId = 999,
            VehicleId = null,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            TimeStart = new TimeOnly(10, 0),
            TimeEnd = new TimeOnly(16, 0),
            Points = new List<DeliveryPointRequest>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _deliveryService.CreateDeliveryAsync(request, 1));
    }

    [Fact]
    public async Task DeleteDeliveryAsync_WithPastDelivery_ThrowsValidationException()
    {
        // Arrange
        var pastDelivery = CreateSampleDelivery(1);
        pastDelivery.DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));

        _deliveryRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pastDelivery);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _deliveryService.DeleteDeliveryAsync(1));
    }

    [Fact]
    public async Task DeleteDeliveryAsync_WithFutureDelivery_Succeeds()
    {
        // Arrange
        var futureDelivery = CreateSampleDelivery(1);
        futureDelivery.DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

        _deliveryRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(futureDelivery);

        _deliveryRepositoryMock.Setup(r => r.DeleteAsync(futureDelivery))
            .Returns(Task.CompletedTask);

        _deliveryRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _deliveryService.DeleteDeliveryAsync(1);

        // Assert
        _deliveryRepositoryMock.Verify(r => r.DeleteAsync(futureDelivery), Times.Once);
    }

    private Delivery CreateSampleDelivery(long id)
    {
        return new Delivery
        {
            Id = id,
            CourierId = 1,
            VehicleId = 1,
            CreatedById = 1,
            DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            TimeStart = new TimeOnly(9, 0),
            TimeEnd = new TimeOnly(17, 0),
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Courier = new User { Id = 1, Login = "courier", Name = "Courier", Role = UserRole.courier, PasswordHash = "hash" },
            Vehicle = new Vehicle { Id = 1, Brand = "Mercedes", LicensePlate = "ABC123", MaxWeight = 1000m, MaxVolume = 10m },
            CreatedBy = new User { Id = 2, Login = "manager", Name = "Manager", Role = UserRole.manager, PasswordHash = "hash" },
            DeliveryPoints = new List<DeliveryPoint>()
        };
    }
}
