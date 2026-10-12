using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(string.Empty, product.Name);
            Assert.Null(product.Description);
            Assert.Equal(0m, product.Price);
            Assert.Null(product.Category);
            Assert.Equal(0, product.StockQuantity);
            Assert.True(product.IsActive);
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Product_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "A test product",
                Price = 99.99m,
                Category = "Electronics",
                StockQuantity = 100,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal("A test product", product.Description);
            Assert.Equal(99.99m, product.Price);
            Assert.Equal("Electronics", product.Category);
            Assert.Equal(100, product.StockQuantity);
            Assert.True(product.IsActive);
            Assert.Equal(now, product.CreatedAt);
            Assert.Equal(now, product.UpdatedAt);
        }

        [Fact]
        public void Product_IsActive_CanBeSetToFalse()
        {
            // Arrange
            var product = new Product { IsActive = true };

            // Act
            product.IsActive = false;

            // Assert
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Product_UpdatedAt_IsNullableDateTime()
        {
            // Arrange
            var product = new Product();

            // Assert
            Assert.Null(product.UpdatedAt);

            // Act
            product.UpdatedAt = DateTime.UtcNow;

            // Assert
            Assert.NotNull(product.UpdatedAt);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(9.99)]
        [InlineData(999.99)]
        [InlineData(9999.99)]
        public void Product_Price_CanBeSetToVariousValues(double priceValue)
        {
            // Arrange
            var product = new Product();
            var price = (decimal)priceValue;

            // Act
            product.Price = price;

            // Assert
            Assert.Equal(price, product.Price);
        }

        [Fact]
        public void Product_StockQuantity_CanBeZero()
        {
            // Arrange
            var product = new Product { StockQuantity = 10 };

            // Act
            product.StockQuantity = 0;

            // Assert
            Assert.Equal(0, product.StockQuantity);
        }
    }
}
