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
            Assert.Equal(string.Empty, product.Description);
            Assert.Equal(0m, product.Price);
            Assert.Equal(string.Empty, product.Category);
            Assert.Equal(0, product.StockQuantity);
            Assert.True(product.IsActive);
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Product_CreatedAt_DefaultsToUtcNow()
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
        public void Product_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
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
            var product = new Product();

            // Act
            product.IsActive = false;

            // Assert
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Product_UpdatedAt_CanBeSetToNull()
        {
            // Arrange
            var product = new Product { UpdatedAt = DateTime.UtcNow };

            // Act
            product.UpdatedAt = null;

            // Assert
            Assert.Null(product.UpdatedAt);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(9999.99)]
        [InlineData(0)]
        public void Product_Price_CanBeSetToVariousValues(double priceDouble)
        {
            // Arrange
            var product = new Product();
            var price = (decimal)priceDouble;

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

        [Fact]
        public void Product_Id_CanBeUpdated()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Id = 99;

            // Assert
            Assert.Equal(99, product.Id);
        }

        [Fact]
        public void Product_Name_CanBeUpdated()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Name = "Updated Name";

            // Assert
            Assert.Equal("Updated Name", product.Name);
        }
    }
}
