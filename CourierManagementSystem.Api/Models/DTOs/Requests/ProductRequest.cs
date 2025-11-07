using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class ProductRequest
{
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Вес должен быть положительным")]
    public decimal Weight { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Длина должна быть положительной")]
    public decimal Length { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Ширина должна быть положительной")]
    public decimal Width { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Высота должна быть положительной")]
    public decimal Height { get; set; }
}
