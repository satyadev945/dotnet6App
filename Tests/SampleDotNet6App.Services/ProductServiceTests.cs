using Microsoft.EntityFrameworkCore;
using Moq;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using Xunit;

namespace SampleDotNet6App.Services.Tests;

public class ProductServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void ProductService_Constructor_InitializesWithContext()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Act
        var service = new ProductService(context);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenProductsExist_ReturnsAllProducts()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", Price = 10m, Stock = 5 },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Stock = 10 }
        );
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var product = new Product { Id = 1, Name = "Test Product", Price = 15m, Stock = 8 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductNotFound_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);

        // Act
        var result = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidProduct_AddsProductToDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);
        var newProduct = new Product { Name = "New Product", Price = 25m, Stock = 15 };

        // Act
        var result = await service.CreateProductAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsCreatedProduct()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);
        var newProduct = new Product { Name = "Created Product", Description = "Test", Price = 30m, Stock = 20 };

        // Act
        var result = await service.CreateProductAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Created Product", result.Name);
        Assert.Equal("Test", result.Description);
        Assert.Equal(30m, result.Price);
        Assert.Equal(20, result.Stock);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductExists_UpdatesProduct()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 10m, Stock = 5 };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);
        var updatedProduct = new Product { Name = "New Name", Description = "New Desc", Price = 15m, Stock = 10 };

        // Act
        var result = await service.UpdateProductAsync(1, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Desc", result.Description);
        Assert.Equal(15m, result.Price);
        Assert.Equal(10, result.Stock);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductNotFound_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);
        var updatedProduct = new Product { Name = "New Name", Price = 15m, Stock = 10 };

        // Act
        var result = await service.UpdateProductAsync(999, updatedProduct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_DeletesProductAndReturnsTrue()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var product = new Product { Id = 1, Name = "To Delete", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Products);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductNotFound_ReturnsFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);

        // Act
        var result = await service.DeleteProductAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateProductAsync_PreservesId()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var existingProduct = new Product { Id = 5, Name = "Original", Price = 10m, Stock = 5 };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);
        var updatedProduct = new Product { Id = 999, Name = "Updated", Price = 20m, Stock = 10 };

        // Act
        var result = await service.UpdateProductAsync(5, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task CreateProductAsync_WithNullDescription_AllowsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ProductService(context);
        var newProduct = new Product { Name = "No Description", Description = null, Price = 25m, Stock = 15 };

        // Act
        var result = await service.CreateProductAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsProductsInCorrectOrder()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.AddRange(
            new Product { Id = 3, Name = "Product C", Price = 30m, Stock = 5 },
            new Product { Id = 1, Name = "Product A", Price = 10m, Stock = 10 },
            new Product { Id = 2, Name = "Product B", Price = 20m, Stock = 15 }
        );
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.GetAllProductsAsync();
        var list = result.ToList();

        // Assert
        Assert.Equal(3, list.Count);
    }
}
