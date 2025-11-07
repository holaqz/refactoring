using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CourierManagementSystem.Api.Controllers;

[ApiController]
[Route("courier")]
[Authorize(Roles = "courier")]
public class CourierController : ControllerBase
{
    private readonly ICourierService _courierService;

    public CourierController(ICourierService courierService)
    {
        _courierService = courierService;
    }

    [HttpGet("deliveries")]
    public async Task<IActionResult> GetDeliveries(
        [FromQuery] DateOnly? date,
        [FromQuery] DeliveryStatus? status,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userIdClaim == null || !long.TryParse(userIdClaim, out var courierId))
        {
            return Unauthorized();
        }

        var deliveries = await _courierService.GetCourierDeliveriesAsync(courierId, date, status, dateFrom, dateTo);
        return Ok(deliveries);
    }

    [HttpGet("deliveries/{id}")]
    public async Task<IActionResult> GetDeliveryById(long id)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userIdClaim == null || !long.TryParse(userIdClaim, out var courierId))
        {
            return Unauthorized();
        }

        var delivery = await _courierService.GetCourierDeliveryByIdAsync(courierId, id);
        return Ok(delivery);
    }
}
