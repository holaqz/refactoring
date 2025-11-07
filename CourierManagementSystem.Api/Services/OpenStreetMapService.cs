using System.Text.Json;

namespace CourierManagementSystem.Api.Services;

public class OpenStreetMapService : IOpenStreetMapService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenStreetMapService> _logger;

    public OpenStreetMapService(HttpClient httpClient, ILogger<OpenStreetMapService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal> CalculateDistanceAsync(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        try
        {
            var url = $"http://router.project-osrm.org/route/v1/driving/{lon1},{lat1};{lon2},{lat2}?overview=false";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            if (jsonDoc.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
            {
                var route = routes[0];
                if (route.TryGetProperty("distance", out var distance))
                {
                    // Distance is in meters, convert to kilometers
                    return distance.GetDecimal() / 1000m;
                }
            }

            // Fallback to Haversine formula if API fails
            return CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to calculate distance via OSRM API, falling back to Haversine formula");
            return CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        }
    }

    private decimal CalculateHaversineDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double R = 6371; // Earth radius in kilometers

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = R * c;

        return (decimal)distance;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
