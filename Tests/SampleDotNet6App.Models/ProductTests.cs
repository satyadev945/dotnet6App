using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(string.Empty, product.Name);
            Assert.True(product.IsActive);
            Assert.True(product.CreatedAt <= DateTime.UtcNow);
            Assert.True(product.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void Product_SetName_ReturnsCorrectName()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Name = "Test Product";

            // Assert
            Assert.Equal("Test Product", product.Name);
        }

        [Fact]
        public void Product_SetDescription_ReturnsCorrectDescription()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Description = "A test description";

            // Assert
            Assert.Equal("A test description", product.Description);
        }

        [Fact]
        public void Product_SetNullDescription_ReturnsNull()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Description = null;

            // Assert
            Assert.Null(product.Description);
        }

        [Fact]
        public void Product_SetPrice_ReturnsCorrectPrice()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = 99.99m;

            // Assert
            Assert.Equal(99.99m, product.Price);
        }

        [Fact]
        public void Product_SetZeroPrice_ReturnsZero()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = 0m;

            // Assert
            Assert.Equal(0m, product.Price);
        }

        [Fact]
        public void Product_SetCategory_ReturnsCorrectCategory()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Category = "Electronics";

            // Assert
            Assert.Equal("Electronics", product.Category);
        }

        [Fact]
        public void Product_SetStockQuantity_ReturnsCorrectQuantity()
        {
            // Arrange
            var product = new Product();

            // Act
            product.StockQuantity = 100;

            // Assert
            Assert.Equal(100, product.StockQuantity);
        }

        [Fact]
        public void Product_SetIsActiveFalse_ReturnsInactive()
        {
            // Arrange
            var product = new Product();

            // Act
            product.IsActive = false;

            // Assert
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Product_SetUpdatedAt_ReturnsCorrectDate()
        {
            // Arrange
            var product = new Product();
            var now = DateTime.UtcNow;

            // Act
            product.UpdatedAt = now;

            // Assert
            Assert.Equal(now, product.UpdatedAt);
        }

        [Fact]
        public void Product_UpdatedAt_DefaultIsNull()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Product_SetId_ReturnsCorrectId()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Id = 42;

            // Assert
            Assert.Equal(42, product.Id);
        }

        [Fact]
        public void Product_FullInitialization_AllPropertiesSet()
        {
            // Arrange & Act
            var createdAt = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-end laptop",
                Price = 1299.99m,
                Category = "Electronics",
                StockQuantity = 50,
                IsActive = true,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Laptop", product.Name);
            Assert.Equal("High-end laptop", product.Description);
            Assert.Equal(1299.99m, product.Price);
            Assert.Equal("Electronics", product.Category);
            Assert.Equal(50, product.StockQuantity);
            Assert.True(product.IsActive);
            Assert.Equal(createdAt, product.CreatedAt);
        }
    }
}
