using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs;

public class VehicleDto
{
    public long Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public decimal MaxWeight { get; set; }
    public decimal MaxVolume { get; set; }

    public static VehicleDto From(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            Brand = vehicle.Brand,
            LicensePlate = vehicle.LicensePlate,
            MaxWeight = vehicle.MaxWeight,
            MaxVolume = vehicle.MaxVolume
        };
    }
}
