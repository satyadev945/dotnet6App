using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Models
{
    public class DtosTests
    {
        // ---- ProductDto Tests ----
        [Fact]
        public void ProductDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new ProductDto();
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Null(dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Null(dto.Category);
            Assert.Equal(0, dto.StockQuantity);
            Assert.False(dto.IsActive);
            Assert.Null(dto.UpdatedAt);
        }

        [Fact]
        public void ProductDto_SetProperties_ReturnsCorrectValues()
        {
            var now = DateTime.UtcNow;
            var dto = new ProductDto
            {
                Id = 5,
                Name = "Widget",
                Description = "A widget",
                Price = 12.50m,
                Category = "Gadgets",
                StockQuantity = 20,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            Assert.Equal(5, dto.Id);
            Assert.Equal("Widget", dto.Name);
            Assert.Equal("A widget", dto.Description);
            Assert.Equal(12.50m, dto.Price);
            Assert.Equal("Gadgets", dto.Category);
            Assert.Equal(20, dto.StockQuantity);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
            Assert.Equal(now, dto.UpdatedAt);
        }

        // ---- CreateProductDto Tests ----
        [Fact]
        public void CreateProductDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new CreateProductDto();
            Assert.Equal(string.Empty, dto.Name);
            Assert.Null(dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Null(dto.Category);
            Assert.Equal(0, dto.StockQuantity);
        }

        [Fact]
        public void CreateProductDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "Description",
                Price = 49.99m,
                Category = "Electronics",
                StockQuantity = 100
            };

            Assert.Equal("New Product", dto.Name);
            Assert.Equal("Description", dto.Description);
            Assert.Equal(49.99m, dto.Price);
            Assert.Equal("Electronics", dto.Category);
            Assert.Equal(100, dto.StockQuantity);
        }

        // ---- UpdateProductDto Tests ----
        [Fact]
        public void UpdateProductDto_DefaultValues_AreNull()
        {
            var dto = new UpdateProductDto();
            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.Null(dto.Price);
            Assert.Null(dto.Category);
            Assert.Null(dto.StockQuantity);
            Assert.Null(dto.IsActive);
        }

        [Fact]
        public void UpdateProductDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new UpdateProductDto
            {
                Name = "Updated",
                Description = "Updated desc",
                Price = 59.99m,
                Category = "Updated Cat",
                StockQuantity = 50,
                IsActive = false
            };

            Assert.Equal("Updated", dto.Name);
            Assert.Equal("Updated desc", dto.Description);
            Assert.Equal(59.99m, dto.Price);
            Assert.Equal("Updated Cat", dto.Category);
            Assert.Equal(50, dto.StockQuantity);
            Assert.False(dto.IsActive);
        }

        // ---- UserDto Tests ----
        [Fact]
        public void UserDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new UserDto();
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
            Assert.Equal(string.Empty, dto.Role);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserDto_SetProperties_ReturnsCorrectValues()
        {
            var now = DateTime.UtcNow;
            var dto = new UserDto
            {
                Id = 3,
                Username = "jdoe",
                Email = "jdoe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now
            };

            Assert.Equal(3, dto.Id);
            Assert.Equal("jdoe", dto.Username);
            Assert.Equal("jdoe@example.com", dto.Email);
            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("Admin", dto.Role);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
        }

        // ---- LoginRequestDto Tests ----
        [Fact]
        public void LoginRequestDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new LoginRequestDto();
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Password);
        }

        [Fact]
        public void LoginRequestDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new LoginRequestDto
            {
                Username = "admin",
                Password = "secret"
            };

            Assert.Equal("admin", dto.Username);
            Assert.Equal("secret", dto.Password);
        }

        // ---- LoginResponseDto Tests ----
        [Fact]
        public void LoginResponseDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new LoginResponseDto();
            Assert.Equal(string.Empty, dto.Token);
            Assert.NotNull(dto.User);
        }

        [Fact]
        public void LoginResponseDto_SetProperties_ReturnsCorrectValues()
        {
            var expiry = DateTime.UtcNow.AddDays(7);
            var userDto = new UserDto { Id = 1, Username = "admin" };
            var dto = new LoginResponseDto
            {
                Token = "jwt.token.here",
                Expiry = expiry,
                User = userDto
            };

            Assert.Equal("jwt.token.here", dto.Token);
            Assert.Equal(expiry, dto.Expiry);
            Assert.Equal(userDto, dto.User);
        }

        // ---- RegisterUserDto Tests ----
        [Fact]
        public void RegisterUserDto_DefaultValues_AreSetCorrectly()
        {
            var dto = new RegisterUserDto();
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        [Fact]
        public void RegisterUserDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Pass123!",
                FirstName = "New",
                LastName = "User"
            };

            Assert.Equal("newuser", dto.Username);
            Assert.Equal("new@example.com", dto.Email);
            Assert.Equal("Pass123!", dto.Password);
            Assert.Equal("New", dto.FirstName);
            Assert.Equal("User", dto.LastName);
        }
    }
}
