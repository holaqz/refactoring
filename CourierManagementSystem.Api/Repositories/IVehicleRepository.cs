using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IVehicleRepository
{
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle> UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
    Task<int> SaveChangesAsync();
}
