namespace CourierManagementSystem.Api.Models.DTOs.Responses;

public class GenerateDeliveriesResponse
{
    public int TotalGenerated { get; set; }
    public Dictionary<DateOnly, GenerationResultByDate> ByDate { get; set; } = new();
}

public class GenerationResultByDate
{
    public int GeneratedCount { get; set; }
    public List<DeliveryDto> Deliveries { get; set; } = new();
    public List<string>? Warnings { get; set; }
}
