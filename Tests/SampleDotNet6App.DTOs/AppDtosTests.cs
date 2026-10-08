using System;
using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.DTOs.Tests
{
    public class AppDtosTests
    {
        // ─── ProductDto ───────────────────────────────────────────────────────────

        [Fact]
        public void ProductDto_DefaultConstructor_SetsDefaultValues()
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
                Price = 9.99m,
                Category = "Gadgets",
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            Assert.Equal(5, dto.Id);
            Assert.Equal("Widget", dto.Name);
            Assert.Equal("A widget", dto.Description);
            Assert.Equal(9.99m, dto.Price);
            Assert.Equal("Gadgets", dto.Category);
            Assert.Equal(10, dto.StockQuantity);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
            Assert.Equal(now, dto.UpdatedAt);
        }

        // ─── CreateProductDto ─────────────────────────────────────────────────────

        [Fact]
        public void CreateProductDto_DefaultConstructor_SetsDefaultValues()
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
                Description = "Desc",
                Price = 19.99m,
                Category = "Cat",
                StockQuantity = 5
            };

            Assert.Equal("New Product", dto.Name);
            Assert.Equal("Desc", dto.Description);
            Assert.Equal(19.99m, dto.Price);
            Assert.Equal("Cat", dto.Category);
            Assert.Equal(5, dto.StockQuantity);
        }

        // ─── UpdateProductDto ─────────────────────────────────────────────────────

        [Fact]
        public void UpdateProductDto_DefaultConstructor_AllNullable()
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
                Price = 29.99m,
                Category = "Updated Cat",
                StockQuantity = 20,
                IsActive = false
            };

            Assert.Equal("Updated", dto.Name);
            Assert.Equal("Updated desc", dto.Description);
            Assert.Equal(29.99m, dto.Price);
            Assert.Equal("Updated Cat", dto.Category);
            Assert.Equal(20, dto.StockQuantity);
            Assert.False(dto.IsActive);
        }

        // ─── UserDto ──────────────────────────────────────────────────────────────

        [Fact]
        public void UserDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new UserDto();
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
            Assert.Equal(string.Empty, dto.Role);
            Assert.False(dto.IsActive);
            Assert.Null(dto.LastLoginAt);
        }

        [Fact]
        public void UserDto_SetProperties_ReturnsCorrectValues()
        {
            var now = DateTime.UtcNow;
            var dto = new UserDto
            {
                Id = 1,
                Username = "alice",
                Email = "alice@example.com",
                FirstName = "Alice",
                LastName = "Smith",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now,
                LastLoginAt = now
            };

            Assert.Equal(1, dto.Id);
            Assert.Equal("alice", dto.Username);
            Assert.Equal("alice@example.com", dto.Email);
            Assert.Equal("Alice", dto.FirstName);
            Assert.Equal("Smith", dto.LastName);
            Assert.Equal("Admin", dto.Role);
            Assert.True(dto.IsActive);
            Assert.Equal(now, dto.CreatedAt);
            Assert.Equal(now, dto.LastLoginAt);
        }

        // ─── RegisterUserDto ──────────────────────────────────────────────────────

        [Fact]
        public void RegisterUserDto_DefaultConstructor_SetsDefaultValues()
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
                Username = "bob",
                Email = "bob@example.com",
                Password = "Secret123!",
                FirstName = "Bob",
                LastName = "Jones"
            };

            Assert.Equal("bob", dto.Username);
            Assert.Equal("bob@example.com", dto.Email);
            Assert.Equal("Secret123!", dto.Password);
            Assert.Equal("Bob", dto.FirstName);
            Assert.Equal("Jones", dto.LastName);
        }

        // ─── LoginDto ─────────────────────────────────────────────────────────────

        [Fact]
        public void LoginDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new LoginDto();
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Password);
        }

        [Fact]
        public void LoginDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new LoginDto
            {
                Username = "charlie",
                Password = "Pass123!"
            };

            Assert.Equal("charlie", dto.Username);
            Assert.Equal("Pass123!", dto.Password);
        }

        // ─── LoginResponseDto ─────────────────────────────────────────────────────

        [Fact]
        public void LoginResponseDto_DefaultConstructor_SetsDefaultValues()
        {
            var dto = new LoginResponseDto();
            Assert.Equal(string.Empty, dto.Token);
        Assert.Equal(string.Empty, dto.Token);
        // User is declared as null! (null-forgiving) so it is actually null by default
        Assert.Null(dto.User);
        }

        [Fact]
        public void LoginResponseDto_SetProperties_ReturnsCorrectValues()
        {
            var now = DateTime.UtcNow;
            var userDto = new UserDto { Id = 1, Username = "alice" };
            var dto = new LoginResponseDto
            {
                Token = "jwt.token.here",
                User = userDto,
                ExpiresAt = now
            };

            Assert.Equal("jwt.token.here", dto.Token);
            Assert.Equal(userDto, dto.User);
            Assert.Equal(now, dto.ExpiresAt);
        }
    }
}
