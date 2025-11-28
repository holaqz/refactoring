using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
}
