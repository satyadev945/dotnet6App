using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
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
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_context, _mapper, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllProductsAsync Tests

        [Fact]
        public async Task GetAllProductsAsync_NoProducts_ReturnsEmptyList()
        {
            // Arrange - empty database

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_WithActiveProducts_ReturnsActiveProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Product A", Price = 10m, IsActive = true },
                new Product { Name = "Product B", Price = 20m, IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllProductsAsync_WithInactiveProducts_ExcludesInactiveProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Active", Price = 10m, IsActive = true },
                new Product { Name = "Inactive", Price = 20m, IsActive = false }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active", result.First().Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProductsOrderedByName()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Zebra", Price = 10m, IsActive = true },
                new Product { Name = "Apple", Price = 20m, IsActive = true },
                new Product { Name = "Mango", Price = 30m, IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _productService.GetAllProductsAsync()).ToList();

            // Assert
            Assert.Equal("Apple", result[0].Name);
            Assert.Equal("Mango", result[1].Name);
            Assert.Equal("Zebra", result[2].Name);
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Name = "FindMe", Price = 15m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FindMe", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange - empty database

            // Act
            var result = await _productService.GetProductByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_InactiveProduct_ReturnsProduct()
        {
            // Arrange - FindAsync doesn't filter by IsActive
            var product = new Product { Name = "InactiveProduct", Price = 5m, IsActive = false };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region CreateProductAsync Tests

        [Fact]
        public async Task CreateProductAsync_ValidDto_ReturnsCreatedProduct()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "A new product",
                Price = 29.99m,
                StockQuantity = 50,
                Category = "Electronics"
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal("A new product", result.Description);
            Assert.Equal(29.99m, result.Price);
            Assert.Equal(50, result.StockQuantity);
            Assert.Equal("Electronics", result.Category);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_SetsIsActiveTrue()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Active Product",
                Price = 10m,
                StockQuantity = 10
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_PersistsToDatabase()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "Persist Product",
                Price = 5m,
                StockQuantity = 1
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal("Persist Product", dbProduct.Name);
        }

        [Fact]
        public async Task CreateProductAsync_ValidDto_SetsCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow;
            var createDto = new CreateProductDto
            {
                Name = "Timed Product",
                Price = 1m,
                StockQuantity = 1
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);
            var after = DateTime.UtcNow;

            // Assert
            var dbProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(dbProduct);
            Assert.True(dbProduct.CreatedAt >= before && dbProduct.CreatedAt <= after);
        }

        #endregion

        #region UpdateProductAsync Tests

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var product = new Product { Name = "Original", Price = 10m, StockQuantity = 5, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto
            {
                Name = "Updated",
                Price = 20m
            };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
            Assert.Equal(20m, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Ghost" };

            // Act
            var result = await _productService.UpdateProductAsync(9999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_PartialUpdate_OnlyUpdatesProvidedFields()
        {
            // Arrange
            var product = new Product
            {
                Name = "Original Name",
                Description = "Original Desc",
                Price = 10m,
                StockQuantity = 5,
                Category = "OldCat",
                IsActive = true
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto
            {
                Name = "New Name"
                // All other fields are null - should not be updated
            };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);
            Assert.Equal(10m, result.Price);
            Assert.Equal(5, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateDescription_UpdatesCorrectly()
        {
            // Arrange
            var product = new Product { Name = "Prod", Description = "Old Desc", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto { Description = "New Desc" };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Desc", result.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateStockQuantity_UpdatesCorrectly()
        {
            // Arrange
            var product = new Product { Name = "Stock Prod", Price = 5m, StockQuantity = 10, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto { StockQuantity = 99 };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateIsActive_UpdatesCorrectly()
        {
            // Arrange
            var product = new Product { Name = "Active Prod", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto { IsActive = false };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdateCategory_UpdatesCorrectly()
        {
            // Arrange
            var product = new Product { Name = "Cat Prod", Price = 5m, Category = "OldCat", IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateProductDto { Category = "NewCat" };

            // Act
            var result = await _productService.UpdateProductAsync(product.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewCat", result.Category);
        }

        [Fact]
        public async Task UpdateProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = new Product { Name = "Time Prod", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var before = DateTime.UtcNow;
            var updateDto = new UpdateProductDto { Name = "Updated Time Prod" };

            // Act
            await _productService.UpdateProductAsync(product.Id, updateDto);
            var after = DateTime.UtcNow;

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
            Assert.NotNull(dbProduct.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }

        #endregion

        #region DeleteProductAsync Tests

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var product = new Product { Name = "ToDelete", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.DeleteProductAsync(product.Id);

            // Assert
            Assert.True(result);
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
            Assert.False(dbProduct.IsActive);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsFalse()
        {
            // Arrange - empty database

            // Act
            var result = await _productService.DeleteProductAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletedProduct_NotReturnedByGetAll()
        {
            // Arrange
            var product = new Product { Name = "SoftDelete", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            await _productService.DeleteProductAsync(product.Id);
            var allProducts = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Empty(allProducts);
        }

        [Fact]
        public async Task DeleteProductAsync_SetsUpdatedAt()
        {
            // Arrange
            var product = new Product { Name = "DeleteTime", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var before = DateTime.UtcNow;

            // Act
            await _productService.DeleteProductAsync(product.Id);
            var after = DateTime.UtcNow;

            // Assert
            var dbProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(dbProduct);
            Assert.NotNull(dbProduct.UpdatedAt);
            Assert.True(dbProduct.UpdatedAt >= before && dbProduct.UpdatedAt <= after);
        }

        #endregion

        #region GetProductsByCategoryAsync Tests

        [Fact]
        public async Task GetProductsByCategoryAsync_ExistingCategory_ReturnsMatchingProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Laptop", Price = 999m, Category = "Electronics", IsActive = true },
                new Product { Name = "Phone", Price = 499m, Category = "Electronics", IsActive = true },
                new Product { Name = "Book", Price = 15m, Category = "Books", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Electronics");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal("Electronics", p.Category));
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_NonExistingCategory_ReturnsEmptyList()
        {
            // Arrange
            _context.Products.Add(new Product { Name = "Widget", Price = 5m, Category = "Gadgets", IsActive = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductsByCategoryAsync("NonExistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_InactiveProducts_ExcludesInactive()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Active", Price = 10m, Category = "Test", IsActive = true },
                new Product { Name = "Inactive", Price = 20m, Category = "Test", IsActive = false }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Test");

            // Assert
            Assert.Single(result);
            Assert.Equal("Active", result.First().Name);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsProductsOrderedByName()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Zebra Product", Price = 10m, Category = "Test", IsActive = true },
                new Product { Name = "Alpha Product", Price = 20m, Category = "Test", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _productService.GetProductsByCategoryAsync("Test")).ToList();

            // Assert
            Assert.Equal("Alpha Product", result[0].Name);
            Assert.Equal("Zebra Product", result[1].Name);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange - empty database

            // Act
            var result = await _productService.GetProductsByCategoryAsync("Electronics");

            // Assert
            Assert.Empty(result);
        }

        #endregion
    }
}
