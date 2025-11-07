using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryPointRepository : IDeliveryPointRepository
{
    private readonly ApplicationDbContext _context;

    public DeliveryPointRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeliveryPoint>> GetByDeliveryOrderBySequenceAsync(long deliveryId)
    {
        return await _context.DeliveryPoints
            .Include(dp => dp.DeliveryPointProducts)
                .ThenInclude(dpp => dpp.Product)
            .Where(dp => dp.DeliveryId == deliveryId)
            .OrderBy(dp => dp.Sequence)
            .ToListAsync();
    }

    public async Task<List<DeliveryPoint>> GetByDeliveryIdAsync(long deliveryId)
    {
        return await _context.DeliveryPoints
            .Where(dp => dp.DeliveryId == deliveryId)
            .ToListAsync();
    }

    public async Task<DeliveryPoint?> GetByDeliveryAndSequenceAsync(long deliveryId, int sequence)
    {
        return await _context.DeliveryPoints
            .Include(dp => dp.DeliveryPointProducts)
                .ThenInclude(dpp => dpp.Product)
            .FirstOrDefaultAsync(dp => dp.DeliveryId == deliveryId && dp.Sequence == sequence);
    }

    public async Task<int?> GetMaxSequenceByDeliveryIdAsync(long deliveryId)
    {
        return await _context.DeliveryPoints
            .Where(dp => dp.DeliveryId == deliveryId)
            .MaxAsync(dp => (int?)dp.Sequence);
    }

    public async Task DeleteByDeliveryIdAsync(long deliveryId)
    {
        var points = await _context.DeliveryPoints
            .Where(dp => dp.DeliveryId == deliveryId)
            .ToListAsync();

        _context.DeliveryPoints.RemoveRange(points);
    }

    public async Task<DeliveryPoint> CreateAsync(DeliveryPoint deliveryPoint)
    {
        _context.DeliveryPoints.Add(deliveryPoint);
        return deliveryPoint;
    }

    public async Task<DeliveryPoint> UpdateAsync(DeliveryPoint deliveryPoint)
    {
        _context.DeliveryPoints.Update(deliveryPoint);
        return deliveryPoint;
    }

    public async Task DeleteAsync(DeliveryPoint deliveryPoint)
    {
        _context.DeliveryPoints.Remove(deliveryPoint);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
