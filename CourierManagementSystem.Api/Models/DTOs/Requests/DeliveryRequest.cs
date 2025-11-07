using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class DeliveryRequest
{
    public long? CourierId { get; set; }

    public long? VehicleId { get; set; }

    [Required(ErrorMessage = "Дата доставки обязательна")]
    public DateOnly DeliveryDate { get; set; }

    [Required(ErrorMessage = "Время начала обязательно")]
    public TimeOnly TimeStart { get; set; }

    [Required(ErrorMessage = "Время окончания обязательно")]
    public TimeOnly TimeEnd { get; set; }

    [Required(ErrorMessage = "Точки маршрута обязательны")]
    [MinLength(1, ErrorMessage = "Точки маршрута обязательны")]
    public List<DeliveryPointRequest> Points { get; set; } = new();
}
