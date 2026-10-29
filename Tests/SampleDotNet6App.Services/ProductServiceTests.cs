using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ProductService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ── Helper ────────────────────────────────────────────────────────────

        private async Task<Product> SeedProductAsync(
            string name = "Test Product",
            string? description = "Test Description",
            decimal price = 9.99m,
            int stock = 100)
        {
            return await _service.CreateAsync(new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock
            });
        }

        // ── GetAllAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithProducts_ReturnsAllProducts()
        {
            // Arrange
            await SeedProductAsync("Product A", price: 10m);
            await SeedProductAsync("Product B", price: 20m);
            await SeedProductAsync("Product C", price: 30m);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WithSingleProduct_ReturnsSingleProduct()
        {
            // Arrange
            await SeedProductAsync("Only Product");

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
        }

        // ── GetByIdAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var created = await SeedProductAsync("Find Me");

            // Act
            var result = await _service.GetByIdAsync(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result!.Id);
            Assert.Equal("Find Me", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_NegativeId_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "New Widget",
                Description = "A brand new widget",
                Price = 49.99m,
                Stock = 200
            };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("New Widget", result.Name);
            Assert.Equal(49.99m, result.Price);
            Assert.Equal(200, result.Stock);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);
            var product = new Product { Name = "Timed Product", Price = 1m, Stock = 1 };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_PersistsToDatabase()
        {
            // Arrange
            var product = new Product { Name = "Persisted", Price = 5m, Stock = 10 };

            // Act
            var result = await _service.CreateAsync(product);
            var fromDb = await _context.Products.FindAsync(result.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("Persisted", fromDb!.Name);
        }

        [Fact]
        public async Task CreateAsync_ProductWithNullDescription_Succeeds()
        {
            // Arrange
            var product = new Product { Name = "No Desc", Description = null, Price = 1m, Stock = 1 };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Description);
        }

        [Fact]
        public async Task CreateAsync_ProductWithZeroPrice_Succeeds()
        {
            // Arrange
            var product = new Product { Name = "Free Item", Price = 0m, Stock = 5 };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0m, result.Price);
        }

        [Fact]
        public async Task CreateAsync_ProductWithZeroStock_Succeeds()
        {
            // Arrange
            var product = new Product { Name = "Out of Stock", Price = 10m, Stock = 0 };

            // Act
            var result = await _service.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Stock);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_ExistingProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var created = await SeedProductAsync("Original Name", price: 10m, stock: 50);
            var update = new Product
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 25.50m,
                Stock = 75
            };

            // Act
            var result = await _service.UpdateAsync(created.Id, update);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result!.Name);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(25.50m, result.Price);
            Assert.Equal(75, result.Stock);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var update = new Product { Name = "Ghost", Price = 1m, Stock = 1 };

            // Act
            var result = await _service.UpdateAsync(9999, update);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_SetsUpdatedAt()
        {
            // Arrange
            var created = await SeedProductAsync();
            var before = DateTime.UtcNow.AddSeconds(-1);
            var update = new Product { Name = "Updated", Price = 1m, Stock = 1 };

            // Act
            var result = await _service.UpdateAsync(created.Id, update);

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.NotNull(result!.UpdatedAt);
            Assert.True(result.UpdatedAt >= before && result.UpdatedAt <= after);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_PersistsChanges()
        {
            // Arrange
            var created = await SeedProductAsync("Before Update");
            var update = new Product { Name = "After Update", Price = 99m, Stock = 5 };

            // Act
            await _service.UpdateAsync(created.Id, update);
            var fromDb = await _context.Products.FindAsync(created.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("After Update", fromDb!.Name);
            Assert.Equal(99m, fromDb.Price);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_UpdatesDescriptionToNull()
        {
            // Arrange
            var created = await SeedProductAsync(description: "Has Description");
            var update = new Product { Name = "No Desc Now", Description = null, Price = 1m, Stock = 1 };

            // Act
            var result = await _service.UpdateAsync(created.Id, update);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result!.Description);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            var created = await SeedProductAsync();

            // Act
            var result = await _service.DeleteAsync(created.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_RemovesFromDatabase()
        {
            // Arrange
            var created = await SeedProductAsync();

            // Act
            await _service.DeleteAsync(created.Id);
            var fromDb = await _context.Products.FindAsync(created.Id);

            // Assert
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ZeroId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_NegativeId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteAsync(-5);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_AfterDeletion_GetByIdReturnsNull()
        {
            // Arrange
            var created = await SeedProductAsync();

            // Act
            await _service.DeleteAsync(created.Id);
            var result = await _service.GetByIdAsync(created.Id);

            // Assert
            Assert.Null(result);
        }
    }
}
