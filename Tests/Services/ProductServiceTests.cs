using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _productService = new ProductService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task SeedProductsAsync()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Description = "A powerful laptop", Price = 999.99m, Category = "Electronics", StockQuantity = 10, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Category = "Accessories", StockQuantity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m, Category = "Accessories", StockQuantity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 4, Name = "OldProduct", Description = "Discontinued item", Price = 9.99m, Category = "Other", StockQuantity = 0, IsActive = false, CreatedAt = DateTime.UtcNow }
            };
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        // ─── GetAllAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.All(result, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsProductsOrderedByName()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = (await _productService.GetAllAsync()).ToList();

            // Assert
            Assert.Equal("Keyboard", result[0].Name);
            Assert.Equal("Laptop", result[1].Name);
            Assert.Equal("Mouse", result[2].Name);
        }

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        // ─── GetByIdAsync ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingActiveProduct_ReturnsProduct()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Laptop", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_InactiveProduct_ReturnsNull()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.GetByIdAsync(4);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // ─── SearchAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task SearchAsync_NullSearchTerm_ReturnsAllActiveProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync(null!);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchAsync_EmptySearchTerm_ReturnsAllActiveProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync(string.Empty);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchAsync_WhitespaceSearchTerm_ReturnsAllActiveProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("   ");

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task SearchAsync_MatchingName_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("laptop");

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop", result.First().Name);
        }

        [Fact]
        public async Task SearchAsync_MatchingDescription_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("wireless");

            // Assert
            Assert.Single(result);
            Assert.Equal("Mouse", result.First().Name);
        }

        [Fact]
        public async Task SearchAsync_MatchingCategory_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("accessories");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_CaseInsensitive_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("LAPTOP");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("nonexistentterm");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_DoesNotReturnInactiveProducts()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.SearchAsync("OldProduct");

            // Assert
            Assert.Empty(result);
        }

        // ─── CreateAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "New Product",
                Description = "A new product",
                Price = 49.99m,
                Category = "Test",
                StockQuantity = 5
            };

            // Act
            var result = await _productService.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_SetsCreatedAt()
        {
            // Arrange
            var product = new Product { Name = "TimeTest", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1 };
            var before = DateTime.UtcNow;

            // Act
            var result = await _productService.CreateAsync(product);
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_SetsIsActiveTrue()
        {
            // Arrange
            var product = new Product { Name = "ActiveTest", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1, IsActive = false };

            // Act
            var result = await _productService.CreateAsync(product);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_PersistsToDatabase()
        {
            // Arrange
            var product = new Product { Name = "PersistTest", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1 };

            // Act
            var result = await _productService.CreateAsync(product);
            var fromDb = await _context.Products.FindAsync(result.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("PersistTest", fromDb.Name);
        }

        // ─── UpdateAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_ExistingActiveProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            await SeedProductsAsync();
            var update = new Product { Name = "Updated Laptop", Description = "Updated desc", Price = 1199.99m, Category = "Electronics", StockQuantity = 8 };

            // Act
            var result = await _productService.UpdateAsync(1, update);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Laptop", result.Name);
            Assert.Equal(1199.99m, result.Price);
            Assert.Equal(8, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateAsync_ExistingActiveProduct_SetsUpdatedAt()
        {
            // Arrange
            await SeedProductsAsync();
            var update = new Product { Name = "Updated", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1 };
            var before = DateTime.UtcNow;

            // Act
            var result = await _productService.UpdateAsync(1, update);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UpdatedAt);
            Assert.True(result.UpdatedAt >= before && result.UpdatedAt <= after);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentProduct_ReturnsNull()
        {
            // Arrange
            await SeedProductsAsync();
            var update = new Product { Name = "Ghost", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1 };

            // Act
            var result = await _productService.UpdateAsync(999, update);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_InactiveProduct_ReturnsNull()
        {
            // Arrange
            await SeedProductsAsync();
            var update = new Product { Name = "Inactive Update", Description = "desc", Price = 1m, Category = "Test", StockQuantity = 1 };

            // Act
            var result = await _productService.UpdateAsync(4, update);

            // Assert
            Assert.Null(result);
        }

        // ─── DeleteAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_SoftDeletesSetsIsActiveFalse()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            await _productService.DeleteAsync(1);
            var product = await _context.Products.FindAsync(1);

            // Assert
            Assert.NotNull(product);
            Assert.False(product.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_SetsUpdatedAt()
        {
            // Arrange
            await SeedProductsAsync();
            var before = DateTime.UtcNow;

            // Act
            await _productService.DeleteAsync(1);
            var after = DateTime.UtcNow;
            var product = await _context.Products.FindAsync(1);

            // Assert
            Assert.NotNull(product);
            Assert.NotNull(product.UpdatedAt);
            Assert.True(product.UpdatedAt >= before && product.UpdatedAt <= after);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentProduct_ReturnsFalse()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_AlreadyInactiveProduct_ReturnsTrueAndRemainsInactive()
        {
            // Arrange
            await SeedProductsAsync();

            // Act
            var result = await _productService.DeleteAsync(4);

            // Assert
            Assert.True(result);
            var product = await _context.Products.FindAsync(4);
            Assert.NotNull(product);
            Assert.False(product.IsActive);
        }
    }
}
