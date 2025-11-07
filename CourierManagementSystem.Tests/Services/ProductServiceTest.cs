using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;
using CourierManagementSystem.Api.Services;
using Moq;
using Xunit;

namespace CourierManagementSystem.Tests.Services;

public class ProductServiceTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _productService;

    public ProductServiceTest()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Weight = 10m, Length = 30m, Width = 20m, Height = 15m },
            new Product { Id = 2, Name = "Product 2", Weight = 15m, Length = 40m, Width = 30m, Height = 20m }
        };

        _productRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new ProductRequest
        {
            Name = "New Product",
            Weight = 15.5m,
            Length = 40m,
            Width = 30m,
            Height = 20m
        };

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(15.5m, result.Weight);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Old Product",
            Weight = 10m,
            Length = 30m,
            Width = 20m,
            Height = 15m
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var request = new ProductRequest
        {
            Name = "Updated Product",
            Weight = 20m,
            Length = 50m,
            Width = 40m,
            Height = 30m
        };

        // Act
        var result = await _productService.UpdateProductAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Product", result.Name);
        Assert.Equal(20m, result.Weight);
    }

    [Fact]
    public async Task UpdateProductAsync_WithNonExistentProduct_ThrowsNotFoundException()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var request = new ProductRequest
        {
            Name = "Product",
            Weight = 10m,
            Length = 30m,
            Width = 20m,
            Height = 15m
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateProductAsync(999, request));
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_Succeeds()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Product",
            Weight = 10m,
            Length = 30m,
            Width = 20m,
            Height = 15m
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        _productRepositoryMock.Setup(r => r.DeleteAsync(product))
            .Returns(Task.CompletedTask);

        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _productService.DeleteProductAsync(1);

        // Assert
        _productRepositoryMock.Verify(r => r.DeleteAsync(product), Times.Once);
        _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WithNonExistentProduct_ThrowsNotFoundException()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.DeleteProductAsync(999));
    }
}
