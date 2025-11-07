using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class DeliveryPointRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Порядковый номер должен быть положительным")]
    public int? Sequence { get; set; }

    [Required(ErrorMessage = "Широта обязательна")]
    public decimal Latitude { get; set; }

    [Required(ErrorMessage = "Долгота обязательна")]
    public decimal Longitude { get; set; }

    public List<DeliveryProductRequest> Products { get; set; } = new();
}
