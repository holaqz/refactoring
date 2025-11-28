using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryPointRepository : Repository<DeliveryPoint>, IDeliveryPointRepository
{
    public DeliveryPointRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task DeleteByDeliveryIdAsync(long deliveryId)
    {
        var points = await _context.DeliveryPoints
            .Where(dp => dp.DeliveryId == deliveryId)
            .ToListAsync();

        _context.DeliveryPoints.RemoveRange(points);
    }
}
