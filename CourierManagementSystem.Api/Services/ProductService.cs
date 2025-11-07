using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(ProductDto.From).ToList();
    }

    public async Task<ProductDto> CreateProductAsync(ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Weight = request.Weight,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height
        };

        await _productRepository.CreateAsync(product);
        await _productRepository.SaveChangesAsync();

        return ProductDto.From(product);
    }

    public async Task<ProductDto> UpdateProductAsync(long id, ProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException("Product", id);
        }

        product.Name = request.Name;
        product.Weight = request.Weight;
        product.Length = request.Length;
        product.Width = request.Width;
        product.Height = request.Height;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return ProductDto.From(product);
    }

    public async Task DeleteProductAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException("Product", id);
        }

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();
    }
}
