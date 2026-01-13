using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests;

public class ProductServiceTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void Constructor_WithValidContext_CreatesInstance()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var service = new ProductService(context);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithProducts_ReturnsAllProducts()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Products.AddRange(
            new Product { Name = "Product1", Price = 10m, Stock = 5 },
            new Product { Name = "Product2", Price = 20m, Stock = 10 },
            new Product { Name = "Product3", Price = 30m, Stock = 15 }
        );
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);

        // Act
        var result = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithExistingId_ReturnsProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product { Name = "Test Product", Price = 50m, Stock = 25 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.GetProductByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(50m, result.Price);
        Assert.Equal(25, result.Stock);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidProduct_CreatesAndReturnsProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);
        var product = new Product
        {
            Name = "New Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 100
        };

        // Act
        var result = await service.CreateProductAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Product", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(99.99m, result.Price);
        Assert.Equal(100, result.Stock);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
    }

    [Fact]
    public async Task CreateProductAsync_SetsCreatedDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };

        // Act
        var result = await service.CreateProductAsync(product);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(result.CreatedDate >= beforeCreation);
        Assert.True(result.CreatedDate <= afterCreation);
    }

    [Fact]
    public async Task CreateProductAsync_AddsProductToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };

        // Act
        await service.CreateProductAsync(product);

        // Assert
        Assert.Equal(1, await context.Products.CountAsync());
    }

    [Fact]
    public async Task UpdateProductAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };

        // Act
        var result = await service.UpdateProductAsync(999, product);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsync_WithExistingId_UpdatesAndReturnsProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingProduct = new Product
        {
            Name = "Original",
            Description = "Original Description",
            Price = 50m,
            Stock = 10
        };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        var updatedProduct = new Product
        {
            Name = "Updated",
            Description = "Updated Description",
            Price = 75m,
            Stock = 20
        };

        // Act
        var result = await service.UpdateProductAsync(existingProduct.Id, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(75m, result.Price);
        Assert.Equal(20, result.Stock);
        Assert.NotNull(result.ModifiedDate);
    }

    [Fact]
    public async Task UpdateProductAsync_SetsModifiedDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingProduct = new Product { Name = "Original", Price = 50m, Stock = 10 };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);
        var updatedProduct = new Product { Name = "Updated", Price = 75m, Stock = 20 };

        // Act
        var result = await service.UpdateProductAsync(existingProduct.Id, updatedProduct);
        var afterUpdate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.NotNull(result.ModifiedDate);
        Assert.True(result.ModifiedDate >= beforeUpdate);
        Assert.True(result.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task DeleteProductAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);

        // Act
        var result = await service.DeleteProductAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        var result = await service.DeleteProductAsync(product.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteProductAsync_RemovesProductFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var product = new Product { Name = "Test", Price = 10m, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        // Act
        await service.DeleteProductAsync(product.Id);

        // Assert
        Assert.Equal(0, await context.Products.CountAsync());
    }

    [Fact]
    public async Task CreateProductAsync_WithNullDescription_CreatesProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new ProductService(context);
        var product = new Product
        {
            Name = "Test",
            Description = null,
            Price = 10m,
            Stock = 5
        };

        // Act
        var result = await service.CreateProductAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task UpdateProductAsync_WithZeroPrice_UpdatesProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingProduct = new Product { Name = "Test", Price = 50m, Stock = 10 };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        var updatedProduct = new Product { Name = "Test", Price = 0m, Stock = 10 };

        // Act
        var result = await service.UpdateProductAsync(existingProduct.Id, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_WithNegativeStock_UpdatesProduct()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingProduct = new Product { Name = "Test", Price = 50m, Stock = 10 };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();
        var service = new ProductService(context);

        var updatedProduct = new Product { Name = "Test", Price = 50m, Stock = -5 };

        // Act
        var result = await service.UpdateProductAsync(existingProduct.Id, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(-5, result.Stock);
    }
}
