using System.ComponentModel.DataAnnotations;
using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class UserRequest
{
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имя обязательно")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Роль обязательна")]
    public UserRole Role { get; set; }
}
