using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class CourierService : ICourierService
{
    private readonly IDeliveryRepository _deliveryRepository;

    public CourierService(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<List<CourierDeliveryResponse>> GetCourierDeliveriesAsync(
        long courierId,
        DateOnly? date,
        DeliveryStatus? status,
        DateOnly? dateFrom,
        DateOnly? dateTo)
    {
        List<Delivery> deliveries;

        if (dateFrom.HasValue && dateTo.HasValue)
        {
            deliveries = await _deliveryRepository.GetByCourierIdAndDeliveryDateBetweenWithDetailsAsync(
                courierId, dateFrom.Value, dateTo.Value);
        }
        else if (date.HasValue)
        {
            deliveries = await _deliveryRepository.GetByDeliveryDateAndCourierIdWithDetailsAsync(
                date.Value, courierId);
        }
        else
        {
            deliveries = await _deliveryRepository.GetByCourierWithDetailsAsync(courierId);
        }

        if (status.HasValue)
        {
            deliveries = deliveries.Where(d => d.Status == status.Value).ToList();
        }

        return deliveries.Select(d => new CourierDeliveryResponse
        {
            Id = d.Id,
            DeliveryNumber = $"D-{d.Id:D6}",
            DeliveryDate = d.DeliveryDate,
            TimeStart = d.TimeStart,
            TimeEnd = d.TimeEnd,
            Status = d.Status,
            Vehicle = d.Vehicle != null ? VehicleDto.From(d.Vehicle) : null,
            DeliveryPoints = d.DeliveryPoints
                .OrderBy(dp => dp.Sequence)
                .Select(DeliveryPointDto.From)
                .ToList()
        }).ToList();
    }

    public async Task<DeliveryDto> GetCourierDeliveryByIdAsync(long courierId, long deliveryId)
    {
        var delivery = await _deliveryRepository.GetByIdWithDetailsAsync(deliveryId);

        if (delivery == null)
        {
            throw new NotFoundException("Delivery", deliveryId);
        }

        if (delivery.CourierId != courierId)
        {
            throw new ForbiddenException("Доступ к этой доставке запрещен");
        }

        return DeliveryDto.From(delivery);
    }
}
