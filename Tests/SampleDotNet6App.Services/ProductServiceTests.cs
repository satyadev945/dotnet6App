using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Services.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_context, _mapper, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private async Task<Product> SeedProductAsync(
            string name = "Test Product",
            decimal price = 10.00m,
            bool isActive = true,
            string? category = "TestCat",
            string? description = "Test Desc",
            int stock = 5)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Category = category,
                StockQuantity = stock,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // ─── GetAllProductsAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetAllProductsAsync_ReturnsOnlyActiveProducts()
        {
            // Arrange
            await SeedProductAsync("Active1", isActive: true);
            await SeedProductAsync("Active2", isActive: true);
            await SeedProductAsync("Inactive", isActive: false);

            // Act
            var result = (await _service.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductsOrderedByName()
        {
            // Arrange
            await SeedProductAsync("Zebra");
            await SeedProductAsync("Apple");
            await SeedProductAsync("Mango");

            // Act
            var result = (await _service.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Equal("Apple", result[0].Name);
            Assert.Equal("Mango", result[1].Name);
            Assert.Equal("Zebra", result[2].Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsMappedProductDtos()
        {
            // Arrange
            await SeedProductAsync("Laptop", price: 999.99m, category: "Electronics");

            // Act
            var result = (await _service.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.IsType<ProductDto>(result[0]);
            Assert.Equal("Laptop", result[0].Name);
            Assert.Equal(999.99m, result[0].Price);
        }

        // ─── GetProductByIdAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProductDto()
        {
            // Arrange
            var product = await SeedProductAsync("Widget");

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Widget", result!.Name);
            Assert.Equal(product.Id, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetProductByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_InactiveProduct_ReturnsProductDto()
        {
            // Arrange – GetById does NOT filter by IsActive
            var product = await SeedProductAsync("Inactive", isActive: false);

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.IsActive);
        }

        // ─── SearchProductsAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task SearchProductsAsync_MatchesName_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductAsync("Laptop Pro");
            await SeedProductAsync("Wireless Mouse");

            // Act
            var result = (await _service.SearchProductsAsync("Laptop")).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop Pro", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_MatchesDescription_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductAsync("Product A", description: "Contains keyword here");
            await SeedProductAsync("Product B", description: "No match");

            // Act
            var result = (await _service.SearchProductsAsync("keyword")).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Product A", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_MatchesCategory_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductAsync("Item1", category: "Electronics");
            await SeedProductAsync("Item2", category: "Accessories");

            // Act
            var result = (await _service.SearchProductsAsync("Electronics")).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Item1", result[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            await SeedProductAsync("Laptop");

            // Act
            var result = await _service.SearchProductsAsync("XYZ_NO_MATCH");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchProductsAsync_InactiveProducts_NotReturned()
        {
            // Arrange
            await SeedProductAsync("Laptop", isActive: false);

            // Act
            var result = await _service.SearchProductsAsync("Laptop");

            // Assert
            Assert.Empty(result);
        }

        // ─── CreateProductAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task CreateProductAsync_ValidDto_ReturnsCreatedProductDto()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Desc",
                Price = 49.99m,
                Category = "NewCat",
                StockQuantity = 10
            };

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(49.99m, result.Price);
            Assert.True(result.IsActive);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task CreateProductAsync_SetsIsActiveTrue()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Product", Price = 1m };

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);
            var createDto = new CreateProductDto { Name = "Product", Price = 1m };

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.CreatedAt >= before);
        }

        [Fact]
        public async Task CreateProductAsync_PersistsToDatabase()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Persisted", Price = 5m };

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            var inDb = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(inDb);
            Assert.Equal("Persisted", inDb!.Name);
        }

        // ─── UpdateProductAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProductAsync_ExistingId_UpdatesAndReturnsDto()
        {
            // Arrange
            var product = await SeedProductAsync("Old Name", price: 10m);
            var updateDto = new UpdateProductDto { Name = "New Name", Price = 20m };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result!.Name);
            Assert.Equal(20m, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "X" };

            // Act
            var result = await _service.UpdateProductAsync(9999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_PartialUpdate_OnlyChangesProvidedFields()
        {
            // Arrange
            var product = await SeedProductAsync("Original", price: 10m, category: "Cat");
            var updateDto = new UpdateProductDto { Name = "Changed" }; // only name

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Changed", result!.Name);
            Assert.Equal(10m, result.Price);
            Assert.Equal("Cat", result.Category);
        }

        [Fact]
        public async Task UpdateProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = await SeedProductAsync("Product");
            var before = DateTime.UtcNow.AddSeconds(-1);
            var updateDto = new UpdateProductDto { Name = "Updated" };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result?.UpdatedAt);
            Assert.True(result!.UpdatedAt >= before);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateIsActive_ChangesActiveStatus()
        {
            // Arrange
            var product = await SeedProductAsync("Product", isActive: true);
            var updateDto = new UpdateProductDto { IsActive = false };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result!.IsActive);
        }

        // ─── DeleteProductAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProductAsync_ExistingId_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var product = await SeedProductAsync("ToDelete");

            // Act
            var result = await _service.DeleteProductAsync(product.Id);

            // Assert
            Assert.True(result);
            var inDb = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(inDb);
            Assert.False(inDb!.IsActive);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteProductAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = await SeedProductAsync("ToDelete");
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            await _service.DeleteProductAsync(product.Id);

            // Assert
            var inDb = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(inDb!.UpdatedAt);
            Assert.True(inDb!.UpdatedAt >= before);
        }
    }
}
