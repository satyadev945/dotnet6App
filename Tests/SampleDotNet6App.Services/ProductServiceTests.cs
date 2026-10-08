using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_context, _loggerMock.Object);
        }

        public void Dispose() => _context.Dispose();

        // ── Helper ────────────────────────────────────────────────────────────

        private async Task<Product> SeedProductAsync(
            string name = "Test Product",
            string category = "Electronics",
            decimal price = 99.99m,
            int stock = 10,
            bool isActive = true,
            string? sku = null,
            string? description = null)
        {
            var product = new Product
            {
                Name = name,
                Category = category,
                Price = price,
                StockQuantity = stock,
                IsActive = isActive,
                SKU = sku,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // ── GetAllProductsAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetAllProductsAsync_ReturnsOnlyActiveProducts()
        {
            await SeedProductAsync("Active", isActive: true);
            await SeedProductAsync("Inactive", isActive: false);

            var result = (await _service.GetAllProductsAsync()).ToList();

            Assert.Single(result);
            Assert.Equal("Active", result[0].Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductsOrderedByName()
        {
            await SeedProductAsync("Zebra Product");
            await SeedProductAsync("Apple Product");

            var result = (await _service.GetAllProductsAsync()).ToList();

            Assert.Equal("Apple Product", result[0].Name);
            Assert.Equal("Zebra Product", result[1].Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _service.GetAllProductsAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_MultipleActiveProducts_ReturnsAll()
        {
            await SeedProductAsync("Product A");
            await SeedProductAsync("Product B");
            await SeedProductAsync("Product C");

            var result = (await _service.GetAllProductsAsync()).ToList();

            Assert.Equal(3, result.Count);
        }

        // ── GetProductByIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetProductByIdAsync_ExistingActiveProduct_ReturnsProduct()
        {
            var seeded = await SeedProductAsync("Find Me");
            var result = await _service.GetProductByIdAsync(seeded.Id);

            Assert.NotNull(result);
            Assert.Equal(seeded.Id, result!.Id);
            Assert.Equal("Find Me", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_InactiveProduct_ReturnsNull()
        {
            var seeded = await SeedProductAsync("Inactive", isActive: false);
            var result = await _service.GetProductByIdAsync(seeded.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistentId_ReturnsNull()
        {
            var result = await _service.GetProductByIdAsync(9999);
            Assert.Null(result);
        }

        // ── GetProductsByCategoryAsync ────────────────────────────────────────

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsMatchingActiveProducts()
        {
            await SeedProductAsync("Laptop", "Electronics");
            await SeedProductAsync("Mouse", "Accessories");
            await SeedProductAsync("Monitor", "Electronics");

            var result = (await _service.GetProductsByCategoryAsync("Electronics")).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal("Electronics", p.Category));
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_NoMatchingCategory_ReturnsEmpty()
        {
            await SeedProductAsync("Laptop", "Electronics");

            var result = await _service.GetProductsByCategoryAsync("Furniture");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ExcludesInactiveProducts()
        {
            await SeedProductAsync("Active", "Electronics", isActive: true);
            await SeedProductAsync("Inactive", "Electronics", isActive: false);

            var result = (await _service.GetProductsByCategoryAsync("Electronics")).ToList();

            Assert.Single(result);
            Assert.Equal("Active", result[0].Name);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsOrderedByName()
        {
            await SeedProductAsync("Zebra", "Electronics");
            await SeedProductAsync("Alpha", "Electronics");

            var result = (await _service.GetProductsByCategoryAsync("Electronics")).ToList();

            Assert.Equal("Alpha", result[0].Name);
            Assert.Equal("Zebra", result[1].Name);
        }

        // ── CreateProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task CreateProductAsync_ValidProduct_CreatesAndReturnsProduct()
        {
            var product = new Product
            {
                Name = "New Product",
                Price = 49.99m,
                Category = "Test",
                StockQuantity = 5
            };

            var result = await _service.CreateProductAsync(product);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("New Product", result.Name);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var product = new Product { Name = "Timed Product", Price = 10m };

            var result = await _service.CreateProductAsync(product);
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateProductAsync_SetsIsActiveToTrue()
        {
            var product = new Product { Name = "Active Product", Price = 10m, IsActive = false };

            var result = await _service.CreateProductAsync(product);

            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_PersistsToDatabase()
        {
            var product = new Product { Name = "Persisted", Price = 10m };

            var result = await _service.CreateProductAsync(product);

            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal("Persisted", dbProduct!.Name);
        }

        // ── UpdateProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_UpdatesAndReturnsProduct()
        {
            var seeded = await SeedProductAsync("Original", price: 10m);

            var update = new Product
            {
                Name = "Updated",
                Description = "New desc",
                Price = 20m,
                Category = "NewCat",
                StockQuantity = 50,
                ImageUrl = "http://img.com",
                SKU = "SKU-NEW"
            };

            var result = await _service.UpdateProductAsync(seeded.Id, update);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Name);
            Assert.Equal("New desc", result.Description);
            Assert.Equal(20m, result.Price);
            Assert.Equal("NewCat", result.Category);
            Assert.Equal(50, result.StockQuantity);
            Assert.Equal("http://img.com", result.ImageUrl);
            Assert.Equal("SKU-NEW", result.SKU);
            Assert.NotNull(result.UpdatedAt);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistentProduct_ReturnsNull()
        {
            var update = new Product { Name = "X", Price = 1m };
            var result = await _service.UpdateProductAsync(9999, update);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_SetsUpdatedAtToUtcNow()
        {
            var seeded = await SeedProductAsync("ToUpdate");
            var before = DateTime.UtcNow.AddSeconds(-1);

            var result = await _service.UpdateProductAsync(seeded.Id,
                new Product { Name = "Updated", Price = 5m });
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.NotNull(result!.UpdatedAt);
            Assert.True(result.UpdatedAt >= before && result.UpdatedAt <= after);
        }

        // ── DeleteProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsTrueAndSoftDeletes()
        {
            var seeded = await SeedProductAsync("ToDelete");

            var result = await _service.DeleteProductAsync(seeded.Id);

            Assert.True(result);
            var dbProduct = await _context.Products.FindAsync(seeded.Id);
            Assert.NotNull(dbProduct);
            Assert.False(dbProduct!.IsActive);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistentProduct_ReturnsFalse()
        {
            var result = await _service.DeleteProductAsync(9999);
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_SetsUpdatedAt()
        {
            var seeded = await SeedProductAsync("DeletedProduct");
            var before = DateTime.UtcNow.AddSeconds(-1);

            await _service.DeleteProductAsync(seeded.Id);
            var after = DateTime.UtcNow.AddSeconds(1);

            var dbProduct = await _context.Products.FindAsync(seeded.Id);
            Assert.NotNull(dbProduct!.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }

        // ── SearchProductsAsync ───────────────────────────────────────────────

        [Fact]
        public async Task SearchProductsAsync_MatchesProductName()
        {
            await SeedProductAsync("Laptop Pro", description: "A laptop");
            await SeedProductAsync("Wireless Mouse");

            var result = (await _service.SearchProductsAsync("laptop")).ToList();

            Assert.Single(result);
            Assert.Equal("Laptop Pro", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_MatchesDescription()
        {
            await SeedProductAsync("Product A", description: "Contains keyword here");
            await SeedProductAsync("Product B", description: "No match");

            var result = (await _service.SearchProductsAsync("keyword")).ToList();

            Assert.Single(result);
            Assert.Equal("Product A", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_MatchesCategory()
        {
            await SeedProductAsync("Item1", category: "SpecialCategory");
            await SeedProductAsync("Item2", category: "OtherCategory");

            var result = (await _service.SearchProductsAsync("special")).ToList();

            Assert.Single(result);
            Assert.Equal("Item1", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_MatchesSKU()
        {
            await SeedProductAsync("SKU Product", sku: "UNIQUE-SKU-123");
            await SeedProductAsync("Other Product", sku: "OTHER-SKU");

            var result = (await _service.SearchProductsAsync("unique-sku")).ToList();

            Assert.Single(result);
            Assert.Equal("SKU Product", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_IsCaseInsensitive()
        {
            await SeedProductAsync("UPPERCASE PRODUCT");

            var result = (await _service.SearchProductsAsync("uppercase")).ToList();

            Assert.Single(result);
        }

        [Fact]
        public async Task SearchProductsAsync_ExcludesInactiveProducts()
        {
            await SeedProductAsync("Active Match", isActive: true);
            await SeedProductAsync("Inactive Match", isActive: false);

            var result = (await _service.SearchProductsAsync("match")).ToList();

            Assert.Single(result);
            Assert.Equal("Active Match", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_NoMatch_ReturnsEmpty()
        {
            await SeedProductAsync("Laptop");

            var result = await _service.SearchProductsAsync("zzznomatch");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchProductsAsync_ReturnsOrderedByName()
        {
            await SeedProductAsync("Zebra Search");
            await SeedProductAsync("Alpha Search");

            var result = (await _service.SearchProductsAsync("search")).ToList();

            Assert.Equal("Alpha Search", result[0].Name);
            Assert.Equal("Zebra Search", result[1].Name);
        }

        // ── UpdateStockAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task UpdateStockAsync_ExistingProduct_ReturnsTrueAndUpdatesStock()
        {
            var seeded = await SeedProductAsync("Stock Product", stock: 10);

            var result = await _service.UpdateStockAsync(seeded.Id, 99);

            Assert.True(result);
            var dbProduct = await _context.Products.FindAsync(seeded.Id);
            Assert.Equal(99, dbProduct!.StockQuantity);
        }

        [Fact]
        public async Task UpdateStockAsync_NonExistentProduct_ReturnsFalse()
        {
            var result = await _service.UpdateStockAsync(9999, 50);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStockAsync_ZeroQuantity_UpdatesSuccessfully()
        {
            var seeded = await SeedProductAsync("Zero Stock", stock: 100);

            var result = await _service.UpdateStockAsync(seeded.Id, 0);

            Assert.True(result);
            var dbProduct = await _context.Products.FindAsync(seeded.Id);
            Assert.Equal(0, dbProduct!.StockQuantity);
        }

        [Fact]
        public async Task UpdateStockAsync_SetsUpdatedAt()
        {
            var seeded = await SeedProductAsync("UpdatedStock");
            var before = DateTime.UtcNow.AddSeconds(-1);

            await _service.UpdateStockAsync(seeded.Id, 5);
            var after = DateTime.UtcNow.AddSeconds(1);

            var dbProduct = await _context.Products.FindAsync(seeded.Id);
            Assert.NotNull(dbProduct!.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }
    }
}
