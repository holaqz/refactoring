using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class DeliveryPointProductRepository : IDeliveryPointProductRepository
{
    private readonly ApplicationDbContext _context;

    public DeliveryPointProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeliveryPointProduct>> GetByDeliveryPointIdAsync(long deliveryPointId)
    {
        return await _context.DeliveryPointProducts
            .Include(dpp => dpp.Product)
            .Where(dpp => dpp.DeliveryPointId == deliveryPointId)
            .ToListAsync();
    }

    public async Task<List<DeliveryPointProduct>> GetByProductIdAsync(long productId)
    {
        return await _context.DeliveryPointProducts
            .Include(dpp => dpp.DeliveryPoint)
            .Where(dpp => dpp.ProductId == productId)
            .ToListAsync();
    }

    public async Task DeleteByDeliveryPointIdAsync(long deliveryPointId)
    {
        var products = await _context.DeliveryPointProducts
            .Where(dpp => dpp.DeliveryPointId == deliveryPointId)
            .ToListAsync();

        _context.DeliveryPointProducts.RemoveRange(products);
    }

    public async Task<DeliveryPointProduct> CreateAsync(DeliveryPointProduct deliveryPointProduct)
    {
        _context.DeliveryPointProducts.Add(deliveryPointProduct);
        return deliveryPointProduct;
    }

    public async Task<DeliveryPointProduct> UpdateAsync(DeliveryPointProduct deliveryPointProduct)
    {
        _context.DeliveryPointProducts.Update(deliveryPointProduct);
        return deliveryPointProduct;
    }

    public async Task DeleteAsync(DeliveryPointProduct deliveryPointProduct)
    {
        _context.DeliveryPointProducts.Remove(deliveryPointProduct);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
