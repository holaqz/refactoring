using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryRepository
{
    Task<List<Delivery>> GetAllAsync();
    Task<Delivery?> GetByIdAsync(long id);
    Task<Delivery?> GetByIdWithDetailsAsync(long id);
    Task<List<Delivery>> GetByDeliveryDateWithDetailsAsync(DateOnly date);
    Task<List<Delivery>> GetByCourierWithDetailsAsync(long courierId);
    Task<List<Delivery>> GetByStatusWithDetailsAsync(DeliveryStatus status);
    Task<List<Delivery>> GetByDeliveryDateAndCourierIdWithDetailsAsync(DateOnly date, long courierId);
    Task<List<Delivery>> GetByDeliveryDateAndStatusWithDetailsAsync(DateOnly date, DeliveryStatus status);
    Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(long courierId, DateOnly startDate, DateOnly endDate);
    Task<List<Delivery>> GetByDateVehicleAndOverlappingTimeAsync(DateOnly date, long vehicleId, TimeOnly startTime, TimeOnly endTime, long? excludeDeliveryId = null);
    Task<List<Delivery>> GetByProductIdAsync(long productId);
    Task<Delivery> CreateAsync(Delivery delivery);
    Task<Delivery> UpdateAsync(Delivery delivery);
    Task DeleteAsync(Delivery delivery);
    Task<int> SaveChangesAsync();
}
