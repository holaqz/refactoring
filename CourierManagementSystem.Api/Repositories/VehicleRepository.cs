using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(long id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
    }

    public async Task<List<Vehicle>> GetByMinCapacityAsync(decimal minWeight, decimal minVolume)
    {
        return await _context.Vehicles
            .Where(v => v.MaxWeight >= minWeight && v.MaxVolume >= minVolume)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetAvailableVehiclesForDateAsync(DateOnly date)
    {
        var assignedVehicleIds = await _context.Deliveries
            .Where(d => d.DeliveryDate == date &&
                       (d.Status == DeliveryStatus.planned || d.Status == DeliveryStatus.in_progress))
            .Select(d => d.VehicleId)
            .Distinct()
            .ToListAsync();

        return await _context.Vehicles
            .Where(v => !assignedVehicleIds.Contains(v.Id))
            .ToListAsync();
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
