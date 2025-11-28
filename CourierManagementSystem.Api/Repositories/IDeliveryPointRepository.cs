using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryPointRepository : IRepository<DeliveryPoint>
{
    Task DeleteByDeliveryIdAsync(long deliveryId);
}
