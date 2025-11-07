using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;

namespace CourierManagementSystem.Api.Services;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllVehiclesAsync();
    Task<VehicleDto> CreateVehicleAsync(VehicleRequest request);
    Task<VehicleDto> UpdateVehicleAsync(long id, VehicleRequest request);
    Task DeleteVehicleAsync(long id);
}
