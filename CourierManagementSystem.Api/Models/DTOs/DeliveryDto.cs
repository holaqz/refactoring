using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs;

public class DeliveryDto
{
    public long Id { get; set; }
    public string? DeliveryNumber { get; set; }
    public UserDto? Courier { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public UserDto CreatedBy { get; set; } = null!;
    public DateOnly DeliveryDate { get; set; }
    public TimeOnly TimeStart { get; set; }
    public TimeOnly TimeEnd { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DeliveryPointDto> DeliveryPoints { get; set; } = new();
    public decimal TotalWeight { get; set; }
    public decimal TotalVolume { get; set; }
    public bool CanEdit { get; set; }

    public static DeliveryDto From(Delivery delivery)
    {
        var allProducts = delivery.DeliveryPoints.SelectMany(dp => dp.DeliveryPointProducts).ToList();
        var totalWeight = allProducts.Sum(p => p.Product.Weight * p.Quantity);
        var totalVolume = allProducts.Sum(p => p.Product.GetVolume() * p.Quantity);
        var canEdit = delivery.DeliveryDate > DateOnly.FromDateTime(DateTime.Now.AddDays(3));

        return new DeliveryDto
        {
            Id = delivery.Id,
            DeliveryNumber = $"DEL-{delivery.DeliveryDate.Year}-{delivery.Id:D3}",
            Courier = delivery.Courier != null ? UserDto.From(delivery.Courier) : null,
            Vehicle = delivery.Vehicle != null ? VehicleDto.From(delivery.Vehicle) : null,
            CreatedBy = UserDto.From(delivery.CreatedBy),
            DeliveryDate = delivery.DeliveryDate,
            TimeStart = delivery.TimeStart,
            TimeEnd = delivery.TimeEnd,
            Status = delivery.Status,
            CreatedAt = delivery.CreatedAt,
            UpdatedAt = delivery.UpdatedAt,
            DeliveryPoints = delivery.DeliveryPoints.Select(DeliveryPointDto.From).ToList(),
            TotalWeight = totalWeight,
            TotalVolume = totalVolume,
            CanEdit = canEdit
        };
    }
}
