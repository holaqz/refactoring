using System.ComponentModel.DataAnnotations;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class DeliveryProductRequest
{
    [Required(ErrorMessage = "ID товара обязателен")]
    public long ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
    public int Quantity { get; set; }
}
