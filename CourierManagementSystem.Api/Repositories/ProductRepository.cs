using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetByNameContainingAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

    public async Task<List<Product>> GetByMaxWeightAsync(decimal maxWeight)
    {
        return await _context.Products
            .Where(p => p.Weight <= maxWeight)
            .ToListAsync();
    }

    public async Task<List<Product>> GetByMaxVolumeAsync(decimal maxVolume)
    {
        // Volume is calculated from dimensions
        return await _context.Products
            .ToListAsync()
            .ContinueWith(task =>
            {
                return task.Result
                    .Where(p => p.GetVolume() <= maxVolume)
                    .ToList();
            });
    }

    public async Task<List<Product>> GetByMaxWeightAndVolumeAsync(decimal maxWeight, decimal maxVolume)
    {
        var products = await _context.Products
            .Where(p => p.Weight <= maxWeight)
            .ToListAsync();

        return products
            .Where(p => p.GetVolume() <= maxVolume)
            .ToList();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
