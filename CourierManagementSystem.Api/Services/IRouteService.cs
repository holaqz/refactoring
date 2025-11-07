using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;

namespace CourierManagementSystem.Api.Services;

public interface IRouteService
{
    Task<RouteCalculationResponse> CalculateRouteAsync(RouteCalculationRequest request);
}
