namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class ErrorResponse
{
    public string? Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? Timestamp { get; set; }
}
