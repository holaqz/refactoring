using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs;

public class DeliveryPointProductDto
{
    public long Id { get; set; }
    public ProductDto Product { get; set; } = null!;
    public int Quantity { get; set; }

    public static DeliveryPointProductDto From(DeliveryPointProduct deliveryPointProduct)
    {
        return new DeliveryPointProductDto
        {
            Id = deliveryPointProduct.Id,
            Product = ProductDto.From(deliveryPointProduct.Product),
            Quantity = deliveryPointProduct.Quantity
        };
    }
}
