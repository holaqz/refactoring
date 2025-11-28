using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly ApplicationDbContext _context;

    private IQueryable<Delivery> GetDeliveriesWithDetails()
    {
        return _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product);
    }

    public DeliveryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Delivery>> GetAllAsync()
    {
        return await _context.Deliveries.ToListAsync();
    }

    public async Task<Delivery?> GetByIdAsync(long id)
    {
        return await _context.Deliveries.FindAsync(id);
    }

    public async Task<Delivery?> GetByIdWithDetailsAsync(long id)
    {
        return await GetDeliveriesWithDetails()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Delivery>> GetByDeliveryDateWithDetailsAsync(DateOnly date)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.DeliveryDate == date)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByCourierWithDetailsAsync(long courierId)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByStatusWithDetailsAsync(DeliveryStatus status)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.Status == status)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndCourierIdWithDetailsAsync(DateOnly date, long courierId)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.DeliveryDate == date && d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndStatusWithDetailsAsync(DateOnly date, DeliveryStatus status)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.DeliveryDate == date && d.Status == status)
            .ToListAsync();
    }


    public async Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(long courierId, DateOnly startDate, DateOnly endDate)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.CourierId == courierId &&
                       d.DeliveryDate >= startDate &&
                       d.DeliveryDate <= endDate)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDateVehicleAndOverlappingTimeAsync(DateOnly date, long vehicleId, TimeOnly startTime, TimeOnly endTime, long? excludeDeliveryId = null)
    {
        var query = _context.Deliveries
            .Where(d => d.DeliveryDate == date &&
                       d.VehicleId == vehicleId &&
                       (d.Status == DeliveryStatus.planned || d.Status == DeliveryStatus.in_progress) &&
                       ((d.TimeStart < endTime && d.TimeEnd > startTime)));

        if (excludeDeliveryId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDeliveryId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Delivery>> GetByProductIdAsync(long productId)
    {
        return await GetDeliveriesWithDetails()
            .Where(d => d.DeliveryPoints.Any(dp =>
                dp.DeliveryPointProducts.Any(dpp => dpp.ProductId == productId)))
            .ToListAsync();
    }

    public async Task<Delivery> CreateAsync(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);
        return delivery;
    }

    public async Task<Delivery> UpdateAsync(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        return delivery;
    }

    public async Task DeleteAsync(Delivery delivery)
    {
        _context.Deliveries.Remove(delivery);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
