using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests
{
    public class ProductTests
    {
        // ── Constructor / default values ──────────────────────────────────────

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
            Assert.Equal(0, product.Stock);
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Product_DefaultConstructor_CreatedAtIsUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var product = new Product();
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.True(product.CreatedAt >= before && product.CreatedAt <= after);
        }

        // ── Property setters ──────────────────────────────────────────────────

        [Fact]
        public void Product_SetId_ReturnsCorrectValue()
        {
            var product = new Product { Id = 10 };
            Assert.Equal(10, product.Id);
        }

        [Fact]
        public void Product_SetName_ReturnsCorrectValue()
        {
            var product = new Product { Name = "Widget" };
            Assert.Equal("Widget", product.Name);
        }

        [Fact]
        public void Product_SetDescription_ReturnsCorrectValue()
        {
            var product = new Product { Description = "A useful widget" };
            Assert.Equal("A useful widget", product.Description);
        }

        [Fact]
        public void Product_SetDescription_CanBeNull()
        {
            var product = new Product { Description = null };
            Assert.Null(product.Description);
        }

        [Fact]
        public void Product_SetPrice_ReturnsCorrectValue()
        {
            var product = new Product { Price = 19.99m };
            Assert.Equal(19.99m, product.Price);
        }

        [Fact]
        public void Product_SetStock_ReturnsCorrectValue()
        {
            var product = new Product { Stock = 250 };
            Assert.Equal(250, product.Stock);
        }

        [Fact]
        public void Product_SetUpdatedAt_ReturnsCorrectValue()
        {
            var now = DateTime.UtcNow;
            var product = new Product { UpdatedAt = now };
            Assert.Equal(now, product.UpdatedAt);
        }

        [Fact]
        public void Product_SetCreatedAt_ReturnsCorrectValue()
        {
            var dt = new DateTime(2024, 3, 10, 8, 0, 0, DateTimeKind.Utc);
            var product = new Product { CreatedAt = dt };
            Assert.Equal(dt, product.CreatedAt);
        }

        // ── Full object initialisation ────────────────────────────────────────

        [Fact]
        public void Product_FullInitialisation_AllPropertiesSet()
        {
            var created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updated = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

            var product = new Product
            {
                Id = 5,
                Name = "Gadget",
                Description = "A cool gadget",
                Price = 99.95m,
                Stock = 10,
                CreatedAt = created,
                UpdatedAt = updated
            };

            Assert.Equal(5, product.Id);
            Assert.Equal("Gadget", product.Name);
            Assert.Equal("A cool gadget", product.Description);
            Assert.Equal(99.95m, product.Price);
            Assert.Equal(10, product.Stock);
            Assert.Equal(created, product.CreatedAt);
            Assert.Equal(updated, product.UpdatedAt);
        }

        // ── Edge cases ────────────────────────────────────────────────────────

        [Fact]
        public void Product_Name_CanBeEmptyString()
        {
            var product = new Product { Name = "" };
            Assert.Equal(string.Empty, product.Name);
        }

        [Fact]
        public void Product_Price_CanBeZero()
        {
            var product = new Product { Price = 0m };
            Assert.Equal(0m, product.Price);
        }

        [Fact]
        public void Product_Stock_CanBeZero()
        {
            var product = new Product { Stock = 0 };
            Assert.Equal(0, product.Stock);
        }

        [Fact]
        public void Product_UpdatedAt_CanBeNull()
        {
            var product = new Product { UpdatedAt = null };
            Assert.Null(product.UpdatedAt);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(9.99)]
        [InlineData(999.99)]
        [InlineData(10000.00)]
        public void Product_Price_VariousValues(double priceDouble)
        {
            var price = (decimal)priceDouble;
            var product = new Product { Price = price };
            Assert.Equal(price, product.Price);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1000)]
        [InlineData(int.MaxValue)]
        public void Product_Stock_VariousValues(int stock)
        {
            var product = new Product { Stock = stock };
            Assert.Equal(stock, product.Stock);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public void Product_Id_VariousValues(int id)
        {
            var product = new Product { Id = id };
            Assert.Equal(id, product.Id);
        }
    }
}
