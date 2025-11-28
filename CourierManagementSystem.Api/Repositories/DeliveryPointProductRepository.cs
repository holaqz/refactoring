using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryPointProductRepository : Repository<DeliveryPointProduct>, IDeliveryPointProductRepository
{
    public DeliveryPointProductRepository(ApplicationDbContext context) : base(context)
    {
        return await _context.SaveChangesAsync();
    }
}
