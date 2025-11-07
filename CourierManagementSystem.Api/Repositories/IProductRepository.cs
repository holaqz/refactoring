using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(long id);
    Task<List<Product>> GetByNameContainingAsync(string name);
    Task<List<Product>> GetByMaxWeightAsync(decimal maxWeight);
    Task<List<Product>> GetByMaxVolumeAsync(decimal maxVolume);
    Task<List<Product>> GetByMaxWeightAndVolumeAsync(decimal maxWeight, decimal maxVolume);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<int> SaveChangesAsync();
}
