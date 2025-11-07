namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class RouteCalculationResponse
{
    public decimal DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public SuggestedTime? SuggestedTime { get; set; }
}

public class SuggestedTime
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}
