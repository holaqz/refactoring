using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class VehicleServiceTest
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTest()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _vehicleService = new VehicleService(_vehicleRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllVehiclesAsync_ReturnsAllVehicles()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "Mercedes", LicensePlate = "ABC123", MaxWeight = 2000m, MaxVolume = 20m },
            new Vehicle { Id = 2, Brand = "Volvo", LicensePlate = "XYZ789", MaxWeight = 3000m, MaxVolume = 30m }
        };

        _vehicleRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(vehicles);

        // Act
        var result = await _vehicleService.GetAllVehiclesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateVehicleAsync_WithValidData_ReturnsCreatedVehicle()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(r => r.ExistsByLicensePlateAsync("ABC123"))
            .ReturnsAsync(false);

        _vehicleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Vehicle>()))
            .ReturnsAsync((Vehicle v) => v);

        _vehicleRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new VehicleRequest
        {
            Brand = "Mercedes",
            LicensePlate = "ABC123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        // Act
        var result = await _vehicleService.CreateVehicleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mercedes", result.Brand);
        Assert.Equal("ABC123", result.LicensePlate);
    }

    [Fact]
    public async Task CreateVehicleAsync_WithDuplicateLicensePlate_ThrowsValidationException()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(r => r.ExistsByLicensePlateAsync("ABC123"))
            .ReturnsAsync(true);

        var request = new VehicleRequest
        {
            Brand = "Mercedes",
            LicensePlate = "ABC123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _vehicleService.CreateVehicleAsync(request));
    }

    [Fact]
    public async Task UpdateVehicleAsync_WithValidData_ReturnsUpdatedVehicle()
    {
        // Arrange
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Brand = "Old Brand",
            LicensePlate = "ABC123",
            MaxWeight = 1000m,
            MaxVolume = 10m
        };

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingVehicle);

        _vehicleRepositoryMock.Setup(r => r.ExistsByLicensePlateAsync("ABC123"))
            .ReturnsAsync(false);

        _vehicleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .ReturnsAsync((Vehicle v) => v);

        _vehicleRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new VehicleRequest
        {
            Brand = "New Brand",
            LicensePlate = "ABC123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        // Act
        var result = await _vehicleService.UpdateVehicleAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Brand", result.Brand);
        Assert.Equal(2000m, result.MaxWeight);
    }

    [Fact]
    public async Task UpdateVehicleAsync_WithNonExistentVehicle_ThrowsNotFoundException()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Vehicle?)null);

        var request = new VehicleRequest
        {
            Brand = "Mercedes",
            LicensePlate = "ABC123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _vehicleService.UpdateVehicleAsync(999, request));
    }

    [Fact]
    public async Task DeleteVehicleAsync_WithValidId_Succeeds()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Id = 1,
            Brand = "Mercedes",
            LicensePlate = "ABC123",
            MaxWeight = 2000m,
            MaxVolume = 20m
        };

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(vehicle);

        _vehicleRepositoryMock.Setup(r => r.DeleteAsync(vehicle))
            .Returns(Task.CompletedTask);

        _vehicleRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _vehicleService.DeleteVehicleAsync(1);

        // Assert
        _vehicleRepositoryMock.Verify(r => r.DeleteAsync(vehicle), Times.Once);
        _vehicleRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteVehicleAsync_WithNonExistentVehicle_ThrowsNotFoundException()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _vehicleService.DeleteVehicleAsync(999));
    }
}
