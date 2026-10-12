using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_context, _mapper, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<Product> SeedProductAsync(
            string name = "Test Product",
            string category = "Electronics",
            decimal price = 99.99m,
            bool isActive = true,
            int stock = 10)
        {
            var product = new Product
            {
                Name = name,
                Description = $"Description for {name}",
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

        // ---- GetAllProductsAsync Tests ----
        [Fact]
        public async Task GetAllProductsAsync_WithActiveProducts_ReturnsActiveProducts()
        {
            // Arrange
            await SeedProductAsync("Active1");
            await SeedProductAsync("Active2");
            await SeedProductAsync("Inactive", isActive: false);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetAllProductsAsync_WithNoProducts_ReturnsEmptyList()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductsOrderedByName()
        {
            // Arrange
            await SeedProductAsync("Zebra");
            await SeedProductAsync("Apple");
            await SeedProductAsync("Mango");

            // Act
            var result = (await _productService.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Equal("Apple", result[0].Name);
            Assert.Equal("Mango", result[1].Name);
            Assert.Equal("Zebra", result[2].Name);
        }

        // ---- GetProductByIdAsync Tests ----
        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
        {
            // Arrange
            var product = await SeedProductAsync("FindMe");

            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FindMe", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _productService.GetProductByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        // ---- SearchProductsAsync Tests ----
        [Fact]
        public async Task SearchProductsAsync_ByName_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductAsync("Laptop Pro");
            await SeedProductAsync("Wireless Mouse");
            await SeedProductAsync("USB Hub");

            // Act
            var result = await _productService.SearchProductsAsync("Laptop");

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Laptop Pro", list[0].Name);
        }

        [Fact]
        public async Task SearchProductsAsync_ByCategory_ReturnsMatchingProducts()
        {
            // Arrange
            await SeedProductAsync("Product1", category: "Electronics");
            await SeedProductAsync("Product2", category: "Accessories");

            // Act
            var result = await _productService.SearchProductsAsync("Electronics");

            // Assert
            var list = result.ToList();
            Assert.Single(list);
        }

        [Fact]
        public async Task SearchProductsAsync_WithNoMatch_ReturnsEmptyList()
        {
            // Arrange
            await SeedProductAsync("Laptop");

            // Act
            var result = await _productService.SearchProductsAsync("XYZNotFound");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchProductsAsync_ExcludesInactiveProducts()
        {
            // Arrange
            await SeedProductAsync("Active Laptop", isActive: true);
            await SeedProductAsync("Inactive Laptop", isActive: false);

            // Act
            var result = await _productService.SearchProductsAsync("Laptop");

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Active Laptop", list[0].Name);
        }

        // ---- GetProductsByCategoryAsync Tests ----
        [Fact]
        public async Task GetProductsByCategoryAsync_WithMatchingCategory_ReturnsProducts()
        {
            // Arrange
            await SeedProductAsync("Laptop", category: "Electronics");
            await SeedProductAsync("Monitor", category: "Electronics");
            await SeedProductAsync("Mouse", category: "Accessories");

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Electronics");

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, p => Assert.Equal("Electronics", p.Category));
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_WithNoMatchingCategory_ReturnsEmptyList()
        {
            // Arrange
            await SeedProductAsync("Laptop", category: "Electronics");

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Furniture");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ExcludesInactiveProducts()
        {
            // Arrange
            await SeedProductAsync("Active", category: "Electronics", isActive: true);
            await SeedProductAsync("Inactive", category: "Electronics", isActive: false);

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Electronics");

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Active", list[0].Name);
        }

        // ---- CreateProductAsync Tests ----
        [Fact]
        public async Task CreateProductAsync_WithValidData_CreatesProduct()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "A new product",
                Price = 29.99m,
                Category = "Gadgets",
                StockQuantity = 50
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(29.99m, result.Price);
            Assert.Equal("Gadgets", result.Category);
            Assert.Equal(50, result.StockQuantity);
        }

        [Fact]
        public async Task CreateProductAsync_SetsIsActiveTrue()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Active Product",
                Price = 10.00m
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);
            var createDto = new CreateProductDto
            {
                Name = "Timestamped Product",
                Price = 10.00m
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.CreatedAt >= before);
        }

        [Fact]
        public async Task CreateProductAsync_PersistsToDatabase()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Persisted Product",
                Price = 15.00m
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal("Persisted Product", dbProduct.Name);
        }

        // ---- UpdateProductAsync Tests ----
        [Fact]
        public async Task UpdateProductAsync_WithValidId_UpdatesProduct()
        {
            // Arrange
            var product = await SeedProductAsync("Original Name");
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Price = 199.99m,
                StockQuantity = 25
            };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal(199.99m, result.Price);
            Assert.Equal(25, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test" };

            // Act
            var result = await _productService.UpdateProductAsync(9999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNullFields_DoesNotOverwrite()
        {
            // Arrange
            var product = await SeedProductAsync("Keep Name", price: 50.00m);
            var updateDto = new UpdateProductDto
            {
                Name = null,
                Price = null
            };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Keep Name", result.Name);
            Assert.Equal(50.00m, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = await SeedProductAsync("Update Timestamp");
            var before = DateTime.UtcNow.AddSeconds(-1);
            var updateDto = new UpdateProductDto { Name = "New Name" };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UpdatedAt);
            Assert.True(result.UpdatedAt >= before);
        }

        [Fact]
        public async Task UpdateProductAsync_CanSetIsActive()
        {
            // Arrange
            var product = await SeedProductAsync("Toggle Active");
            var updateDto = new UpdateProductDto { IsActive = false };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }

        // ---- DeleteProductAsync Tests ----
        [Fact]
        public async Task DeleteProductAsync_WithValidId_SoftDeletesProduct()
        {
            // Arrange
            var product = await SeedProductAsync("Delete Me");
            Assert.True(product.IsActive);

            // Act
            var result = await _productService.DeleteProductAsync(product.Id);

            // Assert
            Assert.True(result);
            var deletedProduct = await _context.Products.FindAsync(product.Id);
            Assert.False(deletedProduct!.IsActive);
        }

        [Fact]
        public async Task DeleteProductAsync_WithInvalidId_ReturnsFalse()
        {
            // Act
            var result = await _productService.DeleteProductAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = await SeedProductAsync("Timestamp Delete");
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            await _productService.DeleteProductAsync(product.Id);

            // Assert
            var deletedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(deletedProduct!.UpdatedAt);
            Assert.True(deletedProduct.UpdatedAt >= before);
        }
    }
}
