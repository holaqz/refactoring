using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync();
    Task<Vehicle?> GetByIdAsync(long id);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<List<Vehicle>> GetByMinCapacityAsync(decimal minWeight, decimal minVolume);
    Task<List<Vehicle>> GetAvailableVehiclesForDateAsync(DateOnly date);
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle> UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
    Task<int> SaveChangesAsync();
}
