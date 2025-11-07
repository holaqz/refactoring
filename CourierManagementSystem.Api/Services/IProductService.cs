using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;

namespace CourierManagementSystem.Api.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> CreateProductAsync(ProductRequest request);
    Task<ProductDto> UpdateProductAsync(long id, ProductRequest request);
    Task DeleteProductAsync(long id);
}
