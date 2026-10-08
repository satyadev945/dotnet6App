using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class ProductTests
    {
        // ── Constructor / Default Values ──────────────────────────────────────

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
            Assert.Null(product.Category);
            Assert.Equal(0, product.StockQuantity);
            Assert.True(product.IsActive);
            Assert.Null(product.UpdatedAt);
            Assert.Null(product.ImageUrl);
            Assert.Null(product.SKU);
        }

        [Fact]
        public void Product_CreatedAt_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var product = new Product();
            var after = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.True(product.CreatedAt >= before && product.CreatedAt <= after);
        }

        // ── Property Setters ──────────────────────────────────────────────────

        [Fact]
        public void Product_SetName_ReturnsCorrectValue()
        {
            var product = new Product { Name = "Test Product" };
            Assert.Equal("Test Product", product.Name);
        }

        [Fact]
        public void Product_SetDescription_ReturnsCorrectValue()
        {
            var product = new Product { Description = "A description" };
            Assert.Equal("A description", product.Description);
        }

        [Fact]
        public void Product_SetPrice_ReturnsCorrectValue()
        {
            var product = new Product { Price = 99.99m };
            Assert.Equal(99.99m, product.Price);
        }

        [Fact]
        public void Product_SetCategory_ReturnsCorrectValue()
        {
            var product = new Product { Category = "Electronics" };
            Assert.Equal("Electronics", product.Category);
        }

        [Fact]
        public void Product_SetStockQuantity_ReturnsCorrectValue()
        {
            var product = new Product { StockQuantity = 100 };
            Assert.Equal(100, product.StockQuantity);
        }

        [Fact]
        public void Product_SetIsActive_False_ReturnsCorrectValue()
        {
            var product = new Product { IsActive = false };
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Product_SetUpdatedAt_ReturnsCorrectValue()
        {
            var now = DateTime.UtcNow;
            var product = new Product { UpdatedAt = now };
            Assert.Equal(now, product.UpdatedAt);
        }

        [Fact]
        public void Product_SetImageUrl_ReturnsCorrectValue()
        {
            var product = new Product { ImageUrl = "https://example.com/img.png" };
            Assert.Equal("https://example.com/img.png", product.ImageUrl);
        }

        [Fact]
        public void Product_SetSKU_ReturnsCorrectValue()
        {
            var product = new Product { SKU = "SKU-001" };
            Assert.Equal("SKU-001", product.SKU);
        }

        [Fact]
        public void Product_SetId_ReturnsCorrectValue()
        {
            var product = new Product { Id = 42 };
            Assert.Equal(42, product.Id);
        }

        // ── Data Annotations Validation ───────────────────────────────────────

        [Fact]
        public void Product_Validation_NameRequired_FailsWhenEmpty()
        {
            var product = new Product { Name = string.Empty, Price = 10m };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(product);
            Validator.TryValidateObject(product, ctx, results, true);

            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Product_Validation_ValidProduct_PassesValidation()
        {
            var product = new Product { Name = "Valid Product", Price = 10m };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(product);
            var isValid = Validator.TryValidateObject(product, ctx, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void Product_ZeroPrice_IsAllowed()
        {
            var product = new Product { Name = "Free Item", Price = 0m };
            Assert.Equal(0m, product.Price);
        }

        [Fact]
        public void Product_NegativePrice_CanBeSet()
        {
            // Model itself doesn't restrict negative; business logic would
            var product = new Product { Name = "Discount", Price = -5m };
            Assert.Equal(-5m, product.Price);
        }

        [Fact]
        public void Product_LargeStockQuantity_CanBeSet()
        {
            var product = new Product { Name = "Bulk", StockQuantity = int.MaxValue };
            Assert.Equal(int.MaxValue, product.StockQuantity);
        }
    }
}
