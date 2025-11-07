using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryPointRepository
{
    Task<List<DeliveryPoint>> GetByDeliveryOrderBySequenceAsync(long deliveryId);
    Task<List<DeliveryPoint>> GetByDeliveryIdAsync(long deliveryId);
    Task<DeliveryPoint?> GetByDeliveryAndSequenceAsync(long deliveryId, int sequence);
    Task<int?> GetMaxSequenceByDeliveryIdAsync(long deliveryId);
    Task DeleteByDeliveryIdAsync(long deliveryId);
    Task<DeliveryPoint> CreateAsync(DeliveryPoint deliveryPoint);
    Task<DeliveryPoint> UpdateAsync(DeliveryPoint deliveryPoint);
    Task DeleteAsync(DeliveryPoint deliveryPoint);
    Task<int> SaveChangesAsync();
}
