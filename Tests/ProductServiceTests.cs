using Xunit;
using SampleDotNet6App.Services;
using SampleDotNet6App.Models;
using SampleDotNet6App.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDotNet6App.Tests
{
    public class ProductServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ProductService(context);

            var product1 = new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 100 };
            var product2 = new Product { Id = 2, Name = "Product 2", Price = 20.99m, Stock = 50 };

            await context.Products.AddRangeAsync(product1, product2);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductById_ValidId_ReturnsProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ProductService(context);

            var product = new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 100 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Product 1", result.Name);
        }

        [Fact]
        public async Task CreateProduct_ValidProduct_AddsProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ProductService(context);

            var newProduct = new Product { Name = "New Product", Price = 15.99m, Stock = 75 };

            // Act
            var result = await service.CreateProductAsync(newProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task UpdateProduct_ValidProduct_UpdatesProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ProductService(context);

            var product = new Product { Id = 1, Name = "Original", Price = 10.99m, Stock = 100 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Act
            product.Name = "Updated";
            var result = await service.UpdateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
        }

        [Fact]
        public async Task DeleteProduct_ValidId_DeletesProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ProductService(context);

            var product = new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 100 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            var deletedProduct = await context.Products.FindAsync(1);
            Assert.Null(deletedProduct);
        }
    }
}
