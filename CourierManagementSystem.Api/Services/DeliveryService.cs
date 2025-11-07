using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDeliveryPointRepository _deliveryPointRepository;
    private readonly IDeliveryPointProductRepository _deliveryPointProductRepository;
    private readonly IOpenStreetMapService _openStreetMapService;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IProductRepository productRepository,
        IDeliveryPointRepository deliveryPointRepository,
        IDeliveryPointProductRepository deliveryPointProductRepository,
        IOpenStreetMapService openStreetMapService)
    {
        _deliveryRepository = deliveryRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _productRepository = productRepository;
        _deliveryPointRepository = deliveryPointRepository;
        _deliveryPointProductRepository = deliveryPointProductRepository;
        _openStreetMapService = openStreetMapService;
    }

    public async Task<List<DeliveryDto>> GetAllDeliveriesAsync(DateOnly? date, long? courierId, DeliveryStatus? status)
    {
        List<Delivery> deliveries;

        if (date.HasValue && courierId.HasValue)
        {
            deliveries = await _deliveryRepository.GetByDeliveryDateAndCourierIdWithDetailsAsync(date.Value, courierId.Value);
        }
        else if (date.HasValue && status.HasValue)
        {
            deliveries = await _deliveryRepository.GetByDeliveryDateAndStatusWithDetailsAsync(date.Value, status.Value);
        }
        else if (date.HasValue)
        {
            deliveries = await _deliveryRepository.GetByDeliveryDateWithDetailsAsync(date.Value);
        }
        else if (courierId.HasValue)
        {
            deliveries = await _deliveryRepository.GetByCourierWithDetailsAsync(courierId.Value);
        }
        else if (status.HasValue)
        {
            deliveries = await _deliveryRepository.GetByStatusWithDetailsAsync(status.Value);
        }
        else
        {
            deliveries = await _deliveryRepository.GetAllAsync();
            // Load details manually for all deliveries
            var deliveryIds = deliveries.Select(d => d.Id).ToList();
            deliveries = new List<Delivery>();
            foreach (var id in deliveryIds)
            {
                var delivery = await _deliveryRepository.GetByIdWithDetailsAsync(id);
                if (delivery != null)
                    deliveries.Add(delivery);
            }
        }

        return deliveries.Select(DeliveryDto.From).ToList();
    }

    public async Task<DeliveryDto> GetDeliveryByIdAsync(long id)
    {
        var delivery = await _deliveryRepository.GetByIdWithDetailsAsync(id);
        if (delivery == null)
        {
            throw new NotFoundException("Delivery", id);
        }

        return DeliveryDto.From(delivery);
    }

    public async Task<DeliveryDto> CreateDeliveryAsync(DeliveryRequest request, long createdByUserId)
    {
        var courier = await EnsureCourierAsync(request.CourierId);
        var vehicle = await EnsureVehicleAsync(request.VehicleId);

        ValidateTimeWindow(request);
        await ValidateRouteTimeAsync(request);
        await ValidateVehicleCapacityAsync(request, vehicle, null);

        var delivery = new Delivery
        {
            CourierId = courier.Id,
            VehicleId = vehicle.Id,
            CreatedById = createdByUserId,
            DeliveryDate = request.DeliveryDate,
            TimeStart = request.TimeStart,
            TimeEnd = request.TimeEnd,
            Status = DeliveryStatus.planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _deliveryRepository.CreateAsync(delivery);
        await _deliveryRepository.SaveChangesAsync();

        await CreateDeliveryPointsAsync(delivery.Id, request.Points);

        var createdDelivery = await _deliveryRepository.GetByIdWithDetailsAsync(delivery.Id);
        return DeliveryDto.From(createdDelivery!);
    }

    public async Task<DeliveryDto> UpdateDeliveryAsync(long id, DeliveryRequest request)
    {
        var delivery = await _deliveryRepository.GetByIdWithDetailsAsync(id);
        if (delivery == null)
        {
            throw new NotFoundException("Delivery", id);
        }

        // Check if delivery can be edited (not within 3 days and not in the past)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysUntilDelivery = delivery.DeliveryDate.DayNumber - today.DayNumber;

        if (daysUntilDelivery < 3)
        {
            throw new ValidationException("Нельзя редактировать доставку менее чем за 3 дня до даты доставки");
        }

        var courier = await EnsureCourierAsync(request.CourierId);
        var vehicle = await EnsureVehicleAsync(request.VehicleId);

        ValidateTimeWindow(request);
        await ValidateRouteTimeAsync(request);
        await ValidateVehicleCapacityAsync(request, vehicle, id);

        delivery.CourierId = courier.Id;
        delivery.VehicleId = vehicle.Id;
        delivery.DeliveryDate = request.DeliveryDate;
        delivery.TimeStart = request.TimeStart;
        delivery.TimeEnd = request.TimeEnd;
        delivery.UpdatedAt = DateTime.UtcNow;

        await _deliveryPointRepository.DeleteByDeliveryIdAsync(delivery.Id);
        await _deliveryPointRepository.SaveChangesAsync();

        await CreateDeliveryPointsAsync(delivery.Id, request.Points);

        await _deliveryRepository.UpdateAsync(delivery);
        await _deliveryRepository.SaveChangesAsync();

        var updatedDelivery = await _deliveryRepository.GetByIdWithDetailsAsync(delivery.Id);
        return DeliveryDto.From(updatedDelivery!);
    }

    public async Task DeleteDeliveryAsync(long id)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(id);
        if (delivery == null)
        {
            throw new NotFoundException("Delivery", id);
        }

        // Check if delivery is in the past
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (delivery.DeliveryDate < today)
        {
            throw new ValidationException("Нельзя удалить доставку из прошлого");
        }

        await _deliveryRepository.DeleteAsync(delivery);
        await _deliveryRepository.SaveChangesAsync();
    }

    public async Task<GenerateDeliveriesResponse> GenerateDeliveriesAsync(GenerateDeliveriesRequest request, long createdByUserId)
    {
        var response = new GenerateDeliveriesResponse
        {
            TotalGenerated = 0,
            ByDate = new Dictionary<DateOnly, GenerationResultByDate>()
        };

        foreach (var (date, routes) in request.DeliveryData)
        {
            var resultByDate = new GenerationResultByDate
            {
                GeneratedCount = 0,
                Deliveries = new List<DeliveryDto>(),
                Warnings = new List<string>()
            };

            var couriers = await _userRepository.GetAllCouriersAsync();
            var vehicles = await _vehicleRepository.GetAllAsync();

            if (!couriers.Any())
            {
                resultByDate.Warnings.Add("Нет доступных курьеров");
            }

            if (!vehicles.Any())
            {
                resultByDate.Warnings.Add("Нет доступных машин");
            }

            var routeIndex = 0;

            foreach (var route in routes)
            {
                if (!couriers.Any() || !vehicles.Any())
                {
                    resultByDate.Warnings.Add("Недостаточно ресурсов для создания доставок");
                    break;
                }

                if (routeIndex >= 9)
                {
                    resultByDate.Warnings.Add("Слишком много маршрутов для одного дня");
                    break;
                }

                var courier = couriers[routeIndex % couriers.Count];
                var vehicle = vehicles[routeIndex % vehicles.Count];

                var timeStart = new TimeOnly(9, 0).AddHours(routeIndex);
                var timeEnd = new TimeOnly(18, 0);

                var points = route.Route
                    .Select((point, index) => new DeliveryPointRequest
                    {
                        Sequence = point.Sequence ?? index + 1,
                        Latitude = point.Latitude,
                        Longitude = point.Longitude,
                        Products = ((point.Products != null && point.Products.Any()) ? point.Products : route.Products)
                            .Select(prod => new DeliveryProductRequest
                            {
                                ProductId = prod.ProductId,
                                Quantity = prod.Quantity
                            })
                            .ToList()
                    })
                    .ToList();

                if (!points.Any() || points.All(p => p.Products == null || !p.Products.Any()))
                {
                    resultByDate.Warnings.Add("Маршрут пропущен: отсутствуют товары для доставки");
                    routeIndex++;
                    continue;
                }

                var deliveryRequest = new DeliveryRequest
                {
                    CourierId = courier.Id,
                    VehicleId = vehicle.Id,
                    DeliveryDate = date,
                    TimeStart = timeStart,
                    TimeEnd = timeEnd,
                    Points = points
                };

                try
                {
                    var delivery = await CreateDeliveryAsync(deliveryRequest, createdByUserId);
                    resultByDate.Deliveries.Add(delivery);
                    resultByDate.GeneratedCount++;
                    response.TotalGenerated++;
                }
                catch (ValidationException vex)
                {
                    resultByDate.Warnings.Add($"Ошибка валидации: {vex.Message}");
                }
                catch (Exception ex)
                {
                    resultByDate.Warnings.Add($"Не удалось создать доставку: {ex.Message}");
                }

                routeIndex++;
            }

            if (resultByDate.Warnings.Count == 0)
            {
                resultByDate.Warnings = null;
            }

            response.ByDate[date] = resultByDate;
        }

        return response;
    }

    private async Task<User> EnsureCourierAsync(long? courierId)
    {
        if (!courierId.HasValue)
        {
            throw new ValidationException("Курьер обязателен для создания доставки");
        }

        var courier = await _userRepository.GetByIdAsync(courierId.Value);
        if (courier == null || courier.Role != UserRole.courier)
        {
            throw new ValidationException("Пользователь не является курьером");
        }

        return courier;
    }

    private async Task<Vehicle> EnsureVehicleAsync(long? vehicleId)
    {
        if (!vehicleId.HasValue)
        {
            throw new ValidationException("Машина обязательна для создания доставки");
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId.Value);
        if (vehicle == null)
        {
            throw new ValidationException("Транспортное средство не найдено");
        }

        return vehicle;
    }

    private static void ValidateTimeWindow(DeliveryRequest request)
    {
        if (request.TimeStart >= request.TimeEnd)
        {
            throw new ValidationException("Время начала должно быть раньше времени окончания");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.DeliveryDate < today)
        {
            throw new ValidationException("Дата доставки не может быть в прошлом");
        }
    }

    private async Task ValidateRouteTimeAsync(DeliveryRequest request)
    {
        if (request.Points.Count < 2)
        {
            return;
        }

        var firstPoint = request.Points.First();
        var lastPoint = request.Points.Last();

        var distance = await _openStreetMapService.CalculateDistanceAsync(
            firstPoint.Latitude,
            firstPoint.Longitude,
            lastPoint.Latitude,
            lastPoint.Longitude);

        const decimal speedKmPerHour = 60m;
        var requiredHours = distance / speedKmPerHour;
        var travelMinutes = (int)Math.Ceiling((double)(requiredHours * 60m));
        var bufferMinutes = request.Points.Count * 30;
        var totalRequiredMinutes = travelMinutes + bufferMinutes;

        var availableMinutes = (int)(request.TimeEnd.ToTimeSpan() - request.TimeStart.ToTimeSpan()).TotalMinutes;

        if (totalRequiredMinutes > availableMinutes)
        {
            throw new ValidationException(
                $"Недостаточно времени для выполнения маршрута. Требуется: {totalRequiredMinutes} мин, доступно: {availableMinutes} мин. Расстояние: {distance} км");
        }
    }

    private async Task ValidateVehicleCapacityAsync(DeliveryRequest request, Vehicle vehicle, long? excludeDeliveryId)
    {
        if (!request.Points.Any())
        {
            throw new ValidationException("Точки маршрута обязательны");
        }

        var productQuantities = new Dictionary<long, int>();

        foreach (var point in request.Points)
        {
            foreach (var productRequest in point.Products)
            {
                if (productRequest.Quantity <= 0)
                {
                    throw new ValidationException("Количество товара должно быть положительным");
                }

                if (productQuantities.ContainsKey(productRequest.ProductId))
                {
                    productQuantities[productRequest.ProductId] += productRequest.Quantity;
                }
                else
                {
                    productQuantities[productRequest.ProductId] = productRequest.Quantity;
                }
            }
        }

        if (productQuantities.Count == 0)
        {
            throw new ValidationException("Товары для доставки не указаны");
        }

        decimal totalWeight = 0m;
        decimal totalVolume = 0m;

        foreach (var kvp in productQuantities)
        {
            var product = await _productRepository.GetByIdAsync(kvp.Key);
            if (product == null)
            {
                throw new ValidationException($"Товар с ID {kvp.Key} не найден");
            }

            totalWeight += product.Weight * kvp.Value;
            totalVolume += product.GetVolume() * kvp.Value;
        }

        decimal existingWeight = 0m;
        decimal existingVolume = 0m;

        var overlappingDeliveries = await _deliveryRepository.GetByDateVehicleAndOverlappingTimeAsync(
            request.DeliveryDate,
            vehicle.Id,
            request.TimeStart,
            request.TimeEnd,
            excludeDeliveryId);

        foreach (var delivery in overlappingDeliveries)
        {
            var details = await _deliveryRepository.GetByIdWithDetailsAsync(delivery.Id);
            if (details == null)
            {
                continue;
            }

            foreach (var point in details.DeliveryPoints)
            {
                foreach (var product in point.DeliveryPointProducts)
                {
                    existingWeight += product.Product.Weight * product.Quantity;
                    existingVolume += product.Product.GetVolume() * product.Quantity;
                }
            }
        }

        var totalRequiredWeight = existingWeight + totalWeight;
        var totalRequiredVolume = existingVolume + totalVolume;

        if (totalRequiredWeight > vehicle.MaxWeight)
        {
            throw new ValidationException(
                $"Превышена грузоподъемность машины в период {request.TimeStart}-{request.TimeEnd}. " +
                $"Максимум: {vehicle.MaxWeight} кг, требуется: {totalRequiredWeight} кг " +
                $"(пересекающиеся доставки: {existingWeight} кг, новые: {totalWeight} кг)");
        }

        if (totalRequiredVolume > vehicle.MaxVolume)
        {
            throw new ValidationException(
                $"Превышен объем машины в период {request.TimeStart}-{request.TimeEnd}. " +
                $"Максимум: {vehicle.MaxVolume} м³, требуется: {totalRequiredVolume} м³ " +
                $"(пересекающиеся доставки: {existingVolume} м³, новые: {totalVolume} м³)");
        }
    }

    private async Task CreateDeliveryPointsAsync(long deliveryId, List<DeliveryPointRequest> points)
    {
        var nextSequence = 1;

        foreach (var pointRequest in points)
        {
            var sequence = pointRequest.Sequence ?? nextSequence;
            nextSequence = sequence + 1;

            var deliveryPoint = new DeliveryPoint
            {
                DeliveryId = deliveryId,
                Sequence = sequence,
                Latitude = pointRequest.Latitude,
                Longitude = pointRequest.Longitude
            };

            await _deliveryPointRepository.CreateAsync(deliveryPoint);
            await _deliveryPointRepository.SaveChangesAsync();

            if (pointRequest.Products.Any())
            {
                foreach (var productRequest in pointRequest.Products)
                {
                    var deliveryPointProduct = new DeliveryPointProduct
                    {
                        DeliveryPointId = deliveryPoint.Id,
                        ProductId = productRequest.ProductId,
                        Quantity = productRequest.Quantity
                    };

                    await _deliveryPointProductRepository.CreateAsync(deliveryPointProduct);
                }

                await _deliveryPointProductRepository.SaveChangesAsync();
            }
        }
    }
}
