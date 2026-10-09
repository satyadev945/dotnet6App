using System;
using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Tests.DTOs
{
    public class ProductDtoTests
    {
        [Fact]
        public void ProductDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new ProductDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Null(dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(0, dto.StockQuantity);
            Assert.Null(dto.Category);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void ProductDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var createdAt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var dto = new ProductDto
            {
                Id = 7,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 149.99m,
                StockQuantity = 30,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(7, dto.Id);
            Assert.Equal("Keyboard", dto.Name);
            Assert.Equal("Mechanical keyboard", dto.Description);
            Assert.Equal(149.99m, dto.Price);
            Assert.Equal(30, dto.StockQuantity);
            Assert.Equal("Electronics", dto.Category);
            Assert.True(dto.IsActive);
            Assert.Equal(createdAt, dto.CreatedAt);
        }

        [Fact]
        public void CreateProductDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new CreateProductDto();

            // Assert
            Assert.Equal(string.Empty, dto.Name);
            Assert.Null(dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(0, dto.StockQuantity);
            Assert.Null(dto.Category);
        }

        [Fact]
        public void CreateProductDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new CreateProductDto
            {
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 49.99m,
                StockQuantity = 200,
                Category = "Electronics"
            };

            // Assert
            Assert.Equal("Mouse", dto.Name);
            Assert.Equal("Wireless mouse", dto.Description);
            Assert.Equal(49.99m, dto.Price);
            Assert.Equal(200, dto.StockQuantity);
            Assert.Equal("Electronics", dto.Category);
        }

        [Fact]
        public void UpdateProductDto_DefaultConstructor_AllNullable()
        {
            // Arrange & Act
            var dto = new UpdateProductDto();

            // Assert
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.Null(dto.Price);
            Assert.Null(dto.StockQuantity);
            Assert.Null(dto.Category);
            Assert.Null(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_SetAllProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new UpdateProductDto
            {
                Name = "Updated Product",
                Description = "Updated description",
                Price = 99.99m,
                StockQuantity = 75,
                Category = "Books",
                IsActive = false
            };

            // Assert
            Assert.Equal("Updated Product", dto.Name);
            Assert.Equal("Updated description", dto.Description);
            Assert.Equal(99.99m, dto.Price);
            Assert.Equal(75, dto.StockQuantity);
            Assert.Equal("Books", dto.Category);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_PartialUpdate_OnlySetFields()
        {
            // Arrange & Act
            var dto = new UpdateProductDto
            {
                Name = "Partial Update",
                Price = 55.00m
            };

            // Assert
            Assert.Equal("Partial Update", dto.Name);
            Assert.Equal(55.00m, dto.Price);
            Assert.Null(dto.Description);
            Assert.Null(dto.StockQuantity);
            Assert.Null(dto.Category);
            Assert.Null(dto.IsActive);
        }

        [Fact]
        public void ProductDto_NullOptionalFields_AreAllowed()
        {
            // Arrange & Act
            var dto = new ProductDto
            {
                Id = 3,
                Name = "Simple Product",
                Description = null,
                Category = null
            };

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
        }

        [Fact]
        public void CreateProductDto_NullOptionalFields_AreAllowed()
        {
            // Arrange & Act
            var dto = new CreateProductDto
            {
                Name = "Basic Product",
                Price = 10.00m,
                StockQuantity = 5
            };

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
        }
    }
}
