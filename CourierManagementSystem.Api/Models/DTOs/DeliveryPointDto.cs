using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs;

public class DeliveryPointDto
{
    public long Id { get; set; }
    public int Sequence { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public List<DeliveryPointProductDto> Products { get; set; } = new();

    public static DeliveryPointDto From(DeliveryPoint deliveryPoint)
    {
        return new DeliveryPointDto
        {
            Id = deliveryPoint.Id,
            Sequence = deliveryPoint.Sequence,
            Latitude = deliveryPoint.Latitude,
            Longitude = deliveryPoint.Longitude,
            Products = deliveryPoint.DeliveryPointProducts.Select(DeliveryPointProductDto.From).ToList()
        };
    }
}
