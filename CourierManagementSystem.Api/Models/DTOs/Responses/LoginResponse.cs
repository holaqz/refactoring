namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int? ExpiresIn { get; set; }
    public UserDto? User { get; set; }
}
