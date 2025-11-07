using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<List<VehicleDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Select(VehicleDto.From).ToList();
    }

    public async Task<VehicleDto> CreateVehicleAsync(VehicleRequest request)
    {
        if (await _vehicleRepository.ExistsByLicensePlateAsync(request.LicensePlate))
        {
            throw new ValidationException($"Транспортное средство с номером '{request.LicensePlate}' уже существует");
        }

        var vehicle = new Vehicle
        {
            Brand = request.Brand,
            LicensePlate = request.LicensePlate,
            MaxWeight = request.MaxWeight,
            MaxVolume = request.MaxVolume
        };

        await _vehicleRepository.CreateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        return VehicleDto.From(vehicle);
    }

    public async Task<VehicleDto> UpdateVehicleAsync(long id, VehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
        {
            throw new NotFoundException("Vehicle", id);
        }

        if (request.LicensePlate != vehicle.LicensePlate &&
            await _vehicleRepository.ExistsByLicensePlateAsync(request.LicensePlate))
        {
            throw new ValidationException($"Транспортное средство с номером '{request.LicensePlate}' уже существует");
        }

        vehicle.Brand = request.Brand;
        vehicle.LicensePlate = request.LicensePlate;
        vehicle.MaxWeight = request.MaxWeight;
        vehicle.MaxVolume = request.MaxVolume;

        await _vehicleRepository.UpdateAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();

        return VehicleDto.From(vehicle);
    }

    public async Task DeleteVehicleAsync(long id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
        {
            throw new NotFoundException("Vehicle", id);
        }

        await _vehicleRepository.DeleteAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
    }
}
