using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class RouteCalculationRequest
{
    [Required(ErrorMessage = "Точки маршрута обязательны")]
    public List<DeliveryPointRequest> Points { get; set; } = new();
}
