using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(0, product.Id);
            Assert.Equal(string.Empty, product.Name);
            Assert.Null(product.Description);
            Assert.Equal(0m, product.Price);
            Assert.Equal(0, product.StockQuantity);
            Assert.Null(product.Category);
            Assert.True(product.IsActive);
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Product_SetId_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Id = 10;

            // Assert
            Assert.Equal(10, product.Id);
        }

        [Fact]
        public void Product_SetName_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Name = "Widget";

            // Assert
            Assert.Equal("Widget", product.Name);
        }

        [Fact]
        public void Product_SetDescription_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Description = "A useful widget";

            // Assert
            Assert.Equal("A useful widget", product.Description);
        }

        [Fact]
        public void Product_SetPrice_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = 19.99m;

            // Assert
            Assert.Equal(19.99m, product.Price);
        }

        [Fact]
        public void Product_SetStockQuantity_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.StockQuantity = 100;

            // Assert
            Assert.Equal(100, product.StockQuantity);
        }

        [Fact]
        public void Product_SetCategory_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Category = "Electronics";

            // Assert
            Assert.Equal("Electronics", product.Category);
        }

        [Fact]
        public void Product_SetIsActive_False_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.IsActive = false;

            // Assert
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Product_SetCreatedAt_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();
            var now = DateTime.UtcNow;

            // Act
            product.CreatedAt = now;

            // Assert
            Assert.Equal(now, product.CreatedAt);
        }

        [Fact]
        public void Product_SetUpdatedAt_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();
            var updatedAt = DateTime.UtcNow;

            // Act
            product.UpdatedAt = updatedAt;

            // Assert
            Assert.Equal(updatedAt, product.UpdatedAt);
        }

        [Fact]
        public void Product_CreatedAt_DefaultIsUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var product = new Product();
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(product.CreatedAt >= before && product.CreatedAt <= after);
        }

        [Fact]
        public void Product_ZeroPrice_IsValid()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = 0m;

            // Assert
            Assert.Equal(0m, product.Price);
        }

        [Fact]
        public void Product_NegativeStockQuantity_IsAllowed()
        {
            // Arrange
            var product = new Product();

            // Act
            product.StockQuantity = -5;

            // Assert
            Assert.Equal(-5, product.StockQuantity);
        }

        [Fact]
        public void Product_FullyPopulated_AllPropertiesCorrect()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var product = new Product
            {
                Id = 5,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                StockQuantity = 50,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(5, product.Id);
            Assert.Equal("Laptop", product.Name);
            Assert.Equal("High-performance laptop", product.Description);
            Assert.Equal(999.99m, product.Price);
            Assert.Equal(50, product.StockQuantity);
            Assert.Equal("Electronics", product.Category);
            Assert.True(product.IsActive);
            Assert.Equal(createdAt, product.CreatedAt);
            Assert.Equal(updatedAt, product.UpdatedAt);
        }
    }
}
