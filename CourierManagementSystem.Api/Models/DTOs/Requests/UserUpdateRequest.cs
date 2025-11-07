using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs.Requests;

public class UserUpdateRequest
{
    public string? Name { get; set; }
    public string? Login { get; set; }
    public UserRole? Role { get; set; }
    public string? Password { get; set; }
}
