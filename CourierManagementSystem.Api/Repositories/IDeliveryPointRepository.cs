using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryPointRepository
{
    Task DeleteByDeliveryIdAsync(long deliveryId);
    Task<DeliveryPoint> CreateAsync(DeliveryPoint deliveryPoint);
    Task<DeliveryPoint> UpdateAsync(DeliveryPoint deliveryPoint);
    Task DeleteAsync(DeliveryPoint deliveryPoint);
    Task<int> SaveChangesAsync();
}
