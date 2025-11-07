namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class ValidationErrorResponse
{
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}
