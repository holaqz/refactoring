using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IDeliveryRepository
{
    Task<List<Delivery>> GetAllAsync();
    Task<Delivery?> GetByIdAsync(long id);
    Task<Delivery?> GetByIdWithDetailsAsync(long id);
    Task<List<Delivery>> GetByDeliveryDateAsync(DateOnly date);
    Task<List<Delivery>> GetByDeliveryDateWithDetailsAsync(DateOnly date);
    Task<List<Delivery>> GetByCourierAsync(long courierId);
    Task<List<Delivery>> GetByCourierWithDetailsAsync(long courierId);
    Task<List<Delivery>> GetByStatusAsync(DeliveryStatus status);
    Task<List<Delivery>> GetByStatusWithDetailsAsync(DeliveryStatus status);
    Task<List<Delivery>> GetByDeliveryDateAndCourierIdAsync(DateOnly date, long courierId);
    Task<List<Delivery>> GetByDeliveryDateAndCourierIdWithDetailsAsync(DateOnly date, long courierId);
    Task<List<Delivery>> GetByDeliveryDateAndStatusAsync(DateOnly date, DeliveryStatus status);
    Task<List<Delivery>> GetByDeliveryDateAndStatusWithDetailsAsync(DateOnly date, DeliveryStatus status);
    Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenAsync(long courierId, DateOnly startDate, DateOnly endDate);
    Task<List<Delivery>> GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(long courierId, DateOnly startDate, DateOnly endDate);
    Task<bool> ExistsCourierTimeConflictAsync(long courierId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeDeliveryId = null);
    Task<List<Delivery>> GetActiveByCourierAndDateAsync(long courierId, DateOnly date);
    Task<List<Delivery>> GetByDateRangeAndFiltersAsync(DateOnly? dateFrom, DateOnly? dateTo, long? courierId, DeliveryStatus? status);
    Task<List<Delivery>> GetByDateOrderByTimeAsync(DateOnly date);
    Task<List<Delivery>> GetByDateVehicleAndOverlappingTimeAsync(DateOnly date, long vehicleId, TimeOnly startTime, TimeOnly endTime, long? excludeDeliveryId = null);
    Task<List<Delivery>> GetByProductIdAsync(long productId);
    Task<Delivery> CreateAsync(Delivery delivery);
    Task<Delivery> UpdateAsync(Delivery delivery);
    Task DeleteAsync(Delivery delivery);
    Task<int> SaveChangesAsync();
}
