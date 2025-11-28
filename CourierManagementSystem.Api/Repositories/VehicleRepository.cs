using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.LicensePlate == licensePlate);
    }
}
