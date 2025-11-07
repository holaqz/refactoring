using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CourierManagementSystem.Api.Controllers;

[ApiController]
[Route("deliveries")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveriesController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpGet]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> GetAllDeliveries(
        [FromQuery] DateOnly? date,
        [FromQuery] long? courierId,
        [FromQuery] DeliveryStatus? status)
    {
        var deliveries = await _deliveryService.GetAllDeliveriesAsync(date, courierId, status);
        return Ok(deliveries);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> GetDeliveryById(long id)
    {
        var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
        return Ok(delivery);
    }

    [HttpPost]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> CreateDelivery([FromBody] DeliveryRequest request)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var delivery = await _deliveryService.CreateDeliveryAsync(request, userId);
        return CreatedAtAction(nameof(GetDeliveryById), new { id = delivery.Id }, delivery);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> UpdateDelivery(long id, [FromBody] DeliveryRequest request)
    {
        var delivery = await _deliveryService.UpdateDeliveryAsync(id, request);
        return Ok(delivery);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> DeleteDelivery(long id)
    {
        await _deliveryService.DeleteDeliveryAsync(id);
        return NoContent();
    }

    [HttpPost("generate")]
    [Authorize(Roles = "manager,admin")]
    public async Task<IActionResult> GenerateDeliveries([FromBody] GenerateDeliveriesRequest request)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var response = await _deliveryService.GenerateDeliveriesAsync(request, userId);
        return Ok(response);
    }
}
