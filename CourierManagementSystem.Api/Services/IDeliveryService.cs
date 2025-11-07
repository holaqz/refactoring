using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;

namespace CourierManagementSystem.Api.Services;

public interface IDeliveryService
{
    Task<List<DeliveryDto>> GetAllDeliveriesAsync(DateOnly? date, long? courierId, DeliveryStatus? status);
    Task<DeliveryDto> GetDeliveryByIdAsync(long id);
    Task<DeliveryDto> CreateDeliveryAsync(DeliveryRequest request, long createdByUserId);
    Task<DeliveryDto> UpdateDeliveryAsync(long id, DeliveryRequest request);
    Task DeleteDeliveryAsync(long id);
    Task<GenerateDeliveriesResponse> GenerateDeliveriesAsync(GenerateDeliveriesRequest request, long createdByUserId);
}
