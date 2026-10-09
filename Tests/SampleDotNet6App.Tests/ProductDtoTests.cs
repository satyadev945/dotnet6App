using System;
using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.DTOs.Tests
{
    /// <summary>
    /// Unit tests for ProductDto and related DTOs.
    /// </summary>
    public class ProductDtoTests
    {
        // ─── ProductDto ────────────────────────────────────────────────────────

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
            var now = DateTime.UtcNow;

            // Act
            var dto = new ProductDto
            {
                Id = 3,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 79.99m,
                StockQuantity = 200,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = now
            };

            // Assert
            Assert.Equal(3, dto.Id);
            Assert.Equal("Keyboard", dto.Name);
            Assert.Equal("Mechanical keyboard", dto.Description);
            Assert.Equal(79.99m, dto.Price);
            Assert.Equal(200, dto.StockQuantity);
            Assert.Equal("Electronics", dto.Category);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
        }

        [Fact]
        public void ProductDto_NullableFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new ProductDto { Description = null, Category = null };

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
        }

        // ─── CreateProductDto ──────────────────────────────────────────────────

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
                Price = 29.99m,
                StockQuantity = 150,
                Category = "Electronics"
            };

            // Assert
            Assert.Equal("Mouse", dto.Name);
            Assert.Equal("Wireless mouse", dto.Description);
            Assert.Equal(29.99m, dto.Price);
            Assert.Equal(150, dto.StockQuantity);
            Assert.Equal("Electronics", dto.Category);
        }

        [Fact]
        public void CreateProductDto_NullableFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new CreateProductDto { Description = null, Category = null };

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
        }

        // ─── UpdateProductDto ──────────────────────────────────────────────────

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
        public void UpdateProductDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new UpdateProductDto
            {
                Name = "Updated Name",
                Description = "Updated description",
                Price = 49.99m,
                StockQuantity = 75,
                Category = "Books",
                IsActive = false
            };

            // Assert
            Assert.Equal("Updated Name", dto.Name);
            Assert.Equal("Updated description", dto.Description);
            Assert.Equal(49.99m, dto.Price);
            Assert.Equal(75, dto.StockQuantity);
            Assert.Equal("Books", dto.Category);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_PartialUpdate_OnlySomePropertiesSet()
        {
            // Arrange & Act
            var dto = new UpdateProductDto
            {
                Price = 99.99m,
                IsActive = true
            };

            // Assert
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.Equal(99.99m, dto.Price);
            Assert.Null(dto.StockQuantity);
            Assert.Null(dto.Category);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_IsActive_CanBeTrue()
        {
            // Arrange & Act
            var dto = new UpdateProductDto { IsActive = true };

            // Assert
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_IsActive_CanBeFalse()
        {
            // Arrange & Act
            var dto = new UpdateProductDto { IsActive = false };

            // Assert
            Assert.False(dto.IsActive);
        }
    }
}
