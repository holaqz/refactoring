using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Responses;

namespace CourierManagementSystem.Api.Services;

public interface ICourierService
{
    Task<List<CourierDeliveryResponse>> GetCourierDeliveriesAsync(long courierId, DateOnly? date, DeliveryStatus? status, DateOnly? dateFrom, DateOnly? dateTo);
    Task<DeliveryDto> GetCourierDeliveryByIdAsync(long courierId, long deliveryId);
}
