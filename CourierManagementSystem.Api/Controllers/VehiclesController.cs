using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourierManagementSystem.Api.Controllers;

[ApiController]
[Route("vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicles()
    {
        var vehicles = await _vehicleService.GetAllVehiclesAsync();
        return Ok(vehicles);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVehicle([FromBody] VehicleRequest request)
    {
        var vehicle = await _vehicleService.CreateVehicleAsync(request);
        return CreatedAtAction(nameof(GetAllVehicles), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateVehicle(long id, [FromBody] VehicleRequest request)
    {
        var vehicle = await _vehicleService.UpdateVehicleAsync(id, request);
        return Ok(vehicle);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteVehicle(long id)
    {
        await _vehicleService.DeleteVehicleAsync(id);
        return NoContent();
    }
}
