using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class ProductControllerTest : BaseIntegrationTest
{
    public ProductControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllProducts_WithManagerToken_ShouldReturnProducts()
    {
        // Arrange
        await CreateProductAsync();

        // Act
        var response = await GetWithAuthAsync("/products", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetAllProducts_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/products");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_ShouldReturnCreatedProduct()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "New Product",
            Weight = 15.5m,
            Length = 40m,
            Width = 30m,
            Height = 20m
        };

        // Act
        var response = await PostWithAuthAsync("/products", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal("New Product", product.Name);
        Assert.Equal(15.5m, product.Weight);
    }

    [Fact]
    public async Task CreateProduct_AsManager_ShouldReturnForbidden()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "New Product",
            Weight = 15.5m,
            Length = 40m,
            Width = 30m,
            Height = 20m
        };

        // Act
        var response = await PostWithAuthAsync("/products", request, ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnUpdatedProduct()
    {
        // Arrange
        var product = await CreateProductAsync("Old Product");

        var request = new ProductRequest
        {
            Name = "Updated Product",
            Weight = 20m,
            Length = 50m,
            Width = 40m,
            Height = 30m
        };

        // Act
        var response = await PutWithAuthAsync($"/products/{product.Id}", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Product", updated.Name);
        Assert.Equal(20m, updated.Weight);
    }

    [Fact]
    public async Task UpdateProduct_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "Test",
            Weight = 10m,
            Length = 30m,
            Width = 20m,
            Height = 15m
        };

        // Act
        var response = await PutWithAuthAsync("/products/99999", request, AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var product = await CreateProductAsync("To Delete");

        // Act
        var response = await DeleteWithAuthAsync($"/products/{product.Id}", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await DeleteWithAuthAsync("/products/99999", AdminToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AsManager_ShouldReturnForbidden()
    {
        // Arrange
        var product = await CreateProductAsync();

        // Act
        var response = await DeleteWithAuthAsync($"/products/{product.Id}", ManagerToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ShouldCalculateVolumeCorrectly()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "Volume Test",
            Weight = 10m,
            Length = 100m,  // 100 cm
            Width = 100m,   // 100 cm
            Height = 100m   // 100 cm
        };

        await PostWithAuthAsync("/products", request, AdminToken);

        // Act
        var response = await GetWithAuthAsync("/products", AdminToken);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        // Assert
        var volumeTestProduct = products?.FirstOrDefault(p => p.Name == "Volume Test");
        Assert.NotNull(volumeTestProduct);
        // Volume should be (100 * 100 * 100) / 1000000 = 1 m³
        Assert.Equal(1m, volumeTestProduct.Volume);
    }
}
