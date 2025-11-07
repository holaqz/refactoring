namespace CourierManagementSystem.Api.Services;

public interface IOpenStreetMapService
{
    Task<decimal> CalculateDistanceAsync(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
}
