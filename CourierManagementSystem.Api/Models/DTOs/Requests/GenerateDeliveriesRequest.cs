using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class GenerateDeliveriesRequest
{
    [Required(ErrorMessage = "Данные для генерации не могут быть пустыми")]
    public Dictionary<DateOnly, List<RouteWithProducts>> DeliveryData { get; set; } = new();
}

public class RouteWithProducts
{
    [Required(ErrorMessage = "Маршрут не может быть пустым")]
    [MinLength(1, ErrorMessage = "Маршрут не может быть пустым")]
    public List<DeliveryPointRequest> Route { get; set; } = new();

    [Required(ErrorMessage = "Товары для доставки обязательны")]
    [MinLength(1, ErrorMessage = "Товары для доставки обязательны")]
    public List<DeliveryProductRequest> Products { get; set; } = new();
}
