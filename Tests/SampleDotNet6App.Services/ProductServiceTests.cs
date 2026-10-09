using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Unit tests for ProductService.
    /// </summary>
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

            var mapperConfig = new MapperConfiguration(
                cfg => cfg.AddProfile<MappingProfile>(),
                NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<ProductService>>();

            _service = new ProductService(_context, _mapper, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── Helper ────────────────────────────────────────────────────────────

        private Product CreateTestProduct(
            string name = "Test Product",
            string category = "Electronics",
            decimal price = 9.99m,
            bool isActive = true)
        {
            var product = new Product
            {
                Name = name,
                Description = "Test description",
                Price = price,
                StockQuantity = 10,
                Category = category,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        // ─── GetAllProductsAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetAllProductsAsync_NoProducts_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_WithActiveProducts_ReturnsActiveProducts()
        {
            // Arrange
            CreateTestProduct("Product A");
            CreateTestProduct("Product B");

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllProductsAsync_WithInactiveProducts_ExcludesInactive()
        {
            // Arrange
            CreateTestProduct("Active Product", isActive: true);
            CreateTestProduct("Inactive Product", isActive: false);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Product", result.First().Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsSortedByName()
        {
            // Arrange
            CreateTestProduct("Zebra");
            CreateTestProduct("Apple");
            CreateTestProduct("Mango");

            // Act
            var result = (await _service.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Equal("Apple", result[0].Name);
            Assert.Equal("Mango", result[1].Name);
            Assert.Equal("Zebra", result[2].Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductDtos_NotNull()
        {
            // Arrange
            CreateTestProduct("Widget");

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.All(result, p => Assert.NotNull(p));
        }

        // ─── GetProductByIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProductDto()
        {
            // Arrange
            var product = CreateTestProduct("FindMe");

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Name);
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
            // Arrange – FindAsync does not filter by IsActive
            var product = CreateTestProduct("Inactive", isActive: false);

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsCorrectPrice()
        {
            // Arrange
            var product = CreateTestProduct("PricedProduct", price: 49.99m);

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(49.99m, result!.Price);
        }

        // ─── CreateProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task CreateProductAsync_ValidDto_ReturnsCreatedProductDto()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "Brand new",
                Price = 14.99m,
                StockQuantity = 30,
                Category = "Books"
            };

            // Act
            var result = await _service.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(14.99m, result.Price);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_PersistsToDatabase()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "Persist Product",
                Price = 5.00m,
                StockQuantity = 5
            };

            // Act
            var result = await _service.CreateProductAsync(dto);

            // Assert
            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal("Persist Product", dbProduct!.Name);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_SetsIsActiveTrue()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Active Product", Price = 1.00m };

            // Act
            var result = await _service.CreateProductAsync(dto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow;
            var dto = new CreateProductDto { Name = "Timed Product", Price = 1.00m };

            // Act
            var result = await _service.CreateProductAsync(dto);
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateProductAsync_NullableFields_CanBeNull()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "Minimal Product",
                Price = 1.00m,
                Description = null,
                Category = null
            };

            // Act
            var result = await _service.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Description);
            Assert.Null(result.Category);
        }

        // ─── UpdateProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_ReturnsUpdatedDto()
        {
            // Arrange
            var product = CreateTestProduct("Original");
            var updateDto = new UpdateProductDto { Name = "Updated" };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Name);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Ghost" };

            // Act
            var result = await _service.UpdateProductAsync(9999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatePrice_PersistsChange()
        {
            // Arrange
            var product = CreateTestProduct("PriceProduct", price: 10.00m);
            var updateDto = new UpdateProductDto { Price = 25.00m };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25.00m, result!.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateStockQuantity_PersistsChange()
        {
            // Arrange
            var product = CreateTestProduct("StockProduct");
            var updateDto = new UpdateProductDto { StockQuantity = 999 };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.Equal(999, dbProduct!.StockQuantity);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateCategory_PersistsChange()
        {
            // Arrange
            var product = CreateTestProduct("CatProduct", category: "Electronics");
            var updateDto = new UpdateProductDto { Category = "Books" };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Books", dbProduct!.Category);
        }

        [Fact]
        public async Task UpdateProductAsync_SetIsActiveFalse_PersistsChange()
        {
            // Arrange
            var product = CreateTestProduct("DeactivateProduct");
            var updateDto = new UpdateProductDto { IsActive = false };

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.False(dbProduct!.IsActive);
        }

        [Fact]
        public async Task UpdateProductAsync_NullFields_DoesNotOverwriteExistingValues()
        {
            // Arrange
            var product = CreateTestProduct("KeepName", price: 50.00m);
            var updateDto = new UpdateProductDto { Price = 75.00m }; // Name is null → should not change

            // Act
            var result = await _service.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.Equal("KeepName", dbProduct!.Name);
            Assert.Equal(75.00m, dbProduct.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = CreateTestProduct("TimedUpdate");
            var before = DateTime.UtcNow;
            var updateDto = new UpdateProductDto { Name = "TimedUpdated" };

            // Act
            await _service.UpdateProductAsync(product.Id, updateDto);
            var after = DateTime.UtcNow;

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct!.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }

        // ─── DeleteProductAsync ────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            var product = CreateTestProduct("DeleteMe");

            // Act
            var result = await _service.DeleteProductAsync(product.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteProductAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_SetsIsActiveFalse()
        {
            // Arrange
            var product = CreateTestProduct("SoftDelete");

            // Act
            await _service.DeleteProductAsync(product.Id);

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
            Assert.False(dbProduct!.IsActive);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_DoesNotRemoveFromDatabase()
        {
            // Arrange
            var product = CreateTestProduct("KeepInDb");

            // Act
            await _service.DeleteProductAsync(product.Id);

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_SetsUpdatedAt()
        {
            // Arrange
            var product = CreateTestProduct("UpdatedOnDelete");
            var before = DateTime.UtcNow;

            // Act
            await _service.DeleteProductAsync(product.Id);
            var after = DateTime.UtcNow;

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct!.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }

        // ─── GetProductsByCategoryAsync ────────────────────────────────────────

        [Fact]
        public async Task GetProductsByCategoryAsync_ExistingCategory_ReturnsMatchingProducts()
        {
            // Arrange
            CreateTestProduct("Laptop", category: "Electronics");
            CreateTestProduct("Phone", category: "Electronics");
            CreateTestProduct("Novel", category: "Books");

            // Act
            var result = await _service.GetProductsByCategoryAsync("Electronics");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal("Electronics", p.Category));
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_NonExistingCategory_ReturnsEmpty()
        {
            // Arrange
            CreateTestProduct("Widget", category: "Electronics");

            // Act
            var result = await _service.GetProductsByCategoryAsync("Furniture");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_InactiveProducts_ExcludesInactive()
        {
            // Arrange
            CreateTestProduct("Active Electronics", category: "Electronics", isActive: true);
            CreateTestProduct("Inactive Electronics", category: "Electronics", isActive: false);

            // Act
            var result = await _service.GetProductsByCategoryAsync("Electronics");

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Electronics", result.First().Name);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsSortedByName()
        {
            // Arrange
            CreateTestProduct("Zebra Book", category: "Books");
            CreateTestProduct("Alpha Book", category: "Books");

            // Act
            var result = (await _service.GetProductsByCategoryAsync("Books")).ToList();

            // Assert
            Assert.Equal("Alpha Book", result[0].Name);
            Assert.Equal("Zebra Book", result[1].Name);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_EmptyDatabase_ReturnsEmpty()
        {
            // Act
            var result = await _service.GetProductsByCategoryAsync("Electronics");

            // Assert
            Assert.Empty(result);
        }
    }
}
