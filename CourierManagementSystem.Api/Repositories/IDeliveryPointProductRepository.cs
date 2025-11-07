using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryPointProductRepository
{
    Task<List<DeliveryPointProduct>> GetByDeliveryPointIdAsync(long deliveryPointId);
    Task<List<DeliveryPointProduct>> GetByProductIdAsync(long productId);
    Task DeleteByDeliveryPointIdAsync(long deliveryPointId);
    Task<DeliveryPointProduct> CreateAsync(DeliveryPointProduct deliveryPointProduct);
    Task<DeliveryPointProduct> UpdateAsync(DeliveryPointProduct deliveryPointProduct);
    Task DeleteAsync(DeliveryPointProduct deliveryPointProduct);
    Task<int> SaveChangesAsync();
}
