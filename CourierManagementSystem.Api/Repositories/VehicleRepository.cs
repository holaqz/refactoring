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

    public async Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        return vehicle;
    }

    public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        return vehicle;
    }

    public async Task DeleteAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
