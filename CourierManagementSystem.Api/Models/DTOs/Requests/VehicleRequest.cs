using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class VehicleRequest
{
    [Required(ErrorMessage = "Марка обязательна")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Номер обязателен")]
    public string LicensePlate { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Максимальный вес должен быть положительным")]
    public decimal MaxWeight { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Максимальный объём должен быть положительным")]
    public decimal MaxVolume { get; set; }
}
