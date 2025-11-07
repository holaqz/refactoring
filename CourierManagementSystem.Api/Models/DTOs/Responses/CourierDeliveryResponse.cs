using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class CourierDeliveryResponse
{
    public long Id { get; set; }
    public string DeliveryNumber { get; set; } = string.Empty;
    public DateOnly DeliveryDate { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }
    public DeliveryStatus Status { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public List<DeliveryPointDto> DeliveryPoints { get; set; } = new();
}
