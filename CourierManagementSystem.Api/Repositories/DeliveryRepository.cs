using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly ApplicationDbContext _context;

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
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Delivery>> GetByDeliveryDateAsync(DateOnly date)
    {
        return await _context.Deliveries
            .Where(d => d.DeliveryDate == date)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateWithDetailsAsync(DateOnly date)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.DeliveryDate == date)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByCourierAsync(long courierId)
    {
        return await _context.Deliveries
            .Where(d => d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByCourierWithDetailsAsync(long courierId)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByStatusAsync(DeliveryStatus status)
    {
        return await _context.Deliveries
            .Where(d => d.Status == status)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByStatusWithDetailsAsync(DeliveryStatus status)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.Status == status)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndCourierIdAsync(DateOnly date, long courierId)
    {
        return await _context.Deliveries
            .Where(d => d.DeliveryDate == date && d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndCourierIdWithDetailsAsync(DateOnly date, long courierId)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.DeliveryDate == date && d.CourierId == courierId)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndStatusAsync(DateOnly date, DeliveryStatus status)
    {
        return await _context.Deliveries
            .Where(d => d.DeliveryDate == date && d.Status == status)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDeliveryDateAndStatusWithDetailsAsync(DateOnly date, DeliveryStatus status)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.DeliveryDate == date && d.Status == status)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenAsync(long courierId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.Deliveries
            .Where(d => d.CourierId == courierId &&
                       d.DeliveryDate >= startDate &&
                       d.DeliveryDate <= endDate)
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(long courierId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .Where(d => d.CourierId == courierId &&
                       d.DeliveryDate >= startDate &&
                       d.DeliveryDate <= endDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsCourierTimeConflictAsync(long courierId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeDeliveryId = null)
    {
        var query = _context.Deliveries
            .Where(d => d.CourierId == courierId &&
                       d.DeliveryDate == date &&
                       (d.Status == DeliveryStatus.planned || d.Status == DeliveryStatus.in_progress) &&
                       ((d.TimeStart < endTime && d.TimeEnd > startTime)));

        if (excludeDeliveryId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDeliveryId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<Delivery>> GetActiveByCourierAndDateAsync(long courierId, DateOnly date)
    {
        return await _context.Deliveries
            .Where(d => d.CourierId == courierId &&
                       d.DeliveryDate == date &&
                       (d.Status == DeliveryStatus.planned || d.Status == DeliveryStatus.in_progress))
            .ToListAsync();
    }

    public async Task<List<Delivery>> GetByDateRangeAndFiltersAsync(DateOnly? dateFrom, DateOnly? dateTo, long? courierId, DeliveryStatus? status)
    {
        var query = _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
            .AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(d => d.DeliveryDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(d => d.DeliveryDate <= dateTo.Value);

        if (courierId.HasValue)
            query = query.Where(d => d.CourierId == courierId.Value);

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<List<Delivery>> GetByDateOrderByTimeAsync(DateOnly date)
    {
        return await _context.Deliveries
            .Where(d => d.DeliveryDate == date)
            .OrderBy(d => d.TimeStart)
            .ThenBy(d => d.TimeEnd)
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
        return await _context.Deliveries
            .Include(d => d.Courier)
            .Include(d => d.Vehicle)
            .Include(d => d.CreatedBy)
            .Include(d => d.DeliveryPoints)
                .ThenInclude(dp => dp.DeliveryPointProducts)
                    .ThenInclude(dpp => dpp.Product)
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
