using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;

namespace CourierManagementSystem.Api.Services;

public class RouteService : IRouteService
{
    private readonly IOpenStreetMapService _openStreetMapService;

    public RouteService(IOpenStreetMapService openStreetMapService)
    {
        _openStreetMapService = openStreetMapService;
    }

    public async Task<RouteCalculationResponse> CalculateRouteAsync(RouteCalculationRequest request)
    {
        if (request.Points == null || request.Points.Count < 2)
        {
            return new RouteCalculationResponse
            {
                DistanceKm = 0,
                DurationMinutes = 0
            };
        }

        decimal totalDistance = 0;

        for (int i = 0; i < request.Points.Count - 1; i++)
        {
            var point1 = request.Points[i];
            var point2 = request.Points[i + 1];

            var distance = await _openStreetMapService.CalculateDistanceAsync(
                point1.Latitude,
                point1.Longitude,
                point2.Latitude,
                point2.Longitude
            );

            totalDistance += distance;
        }

        // Assuming average speed of 40 km/h in urban areas
        const decimal averageSpeedKmh = 40m;
        var durationHours = totalDistance / averageSpeedKmh;
        var durationMinutes = (int)Math.Ceiling(durationHours * 60);

        // Add 5 minutes per delivery point for loading/unloading
        var totalDurationMinutes = durationMinutes + (request.Points.Count * 5);

        var response = new RouteCalculationResponse
        {
            DistanceKm = Math.Round(totalDistance, 2),
            DurationMinutes = totalDurationMinutes
        };

        // Calculate suggested time window
        if (totalDurationMinutes > 0)
        {
            var startTime = TimeOnly.FromDateTime(DateTime.Now);
            var endTime = startTime.AddMinutes(totalDurationMinutes);

            response.SuggestedTime = new SuggestedTime
            {
                Start = startTime,
                End = endTime
            };
        }

        return response;
    }
}
