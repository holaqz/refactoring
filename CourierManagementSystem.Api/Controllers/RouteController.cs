using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourierManagementSystem.Api.Controllers;

[ApiController]
[Route("routes")]
[Authorize]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateRoute([FromBody] RouteCalculationRequest request)
    {
        var response = await _routeService.CalculateRouteAsync(request);
        return Ok(response);
    }
}
