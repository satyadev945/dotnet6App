using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Xunit;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Mappings.Tests
{
    /// <summary>
    /// Unit tests for MappingProfile (AutoMapper configuration).
    /// </summary>
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var config = new MapperConfiguration(
                cfg => cfg.AddProfile<MappingProfile>(),
                NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();
        }

        // ─── Configuration Validity ────────────────────────────────────────────

        [Fact]
        public void MappingProfile_Configuration_CanCreateMapper()
        {
            // Arrange
            var config = new MapperConfiguration(
                cfg => cfg.AddProfile<MappingProfile>(),
                NullLoggerFactory.Instance);

            // Act
            var mapper = config.CreateMapper();

            // Assert
            Assert.NotNull(mapper);
        }

        // ─── Product → ProductDto ──────────────────────────────────────────────

        [Fact]
        public void Map_ProductToProductDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Widget",
                Description = "A widget",
                Price = 9.99m,
                StockQuantity = 50,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Equal(product.Id, dto.Id);
            Assert.Equal(product.Name, dto.Name);
            Assert.Equal(product.Description, dto.Description);
            Assert.Equal(product.Price, dto.Price);
            Assert.Equal(product.StockQuantity, dto.StockQuantity);
            Assert.Equal(product.Category, dto.Category);
            Assert.Equal(product.IsActive, dto.IsActive);
            Assert.Equal(product.CreatedAt, dto.CreatedAt);
        }

        [Fact]
        public void Map_ProductToProductDto_NullableFieldsPreserved()
        {
            // Arrange
            var product = new Product { Name = "Minimal", Description = null, Category = null };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
        }

        // ─── CreateProductDto → Product ────────────────────────────────────────

        [Fact]
        public void Map_CreateProductDtoToProduct_MapsAllProperties()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "Desc",
                Price = 19.99m,
                StockQuantity = 100,
                Category = "Books"
            };

            // Act
            var product = _mapper.Map<Product>(dto);

            // Assert
            Assert.Equal(dto.Name, product.Name);
            Assert.Equal(dto.Description, product.Description);
            Assert.Equal(dto.Price, product.Price);
            Assert.Equal(dto.StockQuantity, product.StockQuantity);
            Assert.Equal(dto.Category, product.Category);
        }

        [Fact]
        public void Map_CreateProductDtoToProduct_NullableFieldsPreserved()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Minimal", Description = null, Category = null };

            // Act
            var product = _mapper.Map<Product>(dto);

            // Assert
            Assert.Null(product.Description);
            Assert.Null(product.Category);
        }

        // ─── UpdateProductDto → Product ────────────────────────────────────────

        [Fact]
        public void Map_UpdateProductDtoToProduct_MapsNonNullProperties()
        {
            // Arrange
            var dto = new UpdateProductDto
            {
                Name = "Updated",
                Price = 29.99m
            };
            var product = new Product { Name = "Original", Price = 9.99m };

            // Act
            _mapper.Map(dto, product);

            // Assert
            Assert.Equal("Updated", product.Name);
            Assert.Equal(29.99m, product.Price);
        }

        [Fact]
        public void Map_UpdateProductDtoToProduct_NullFieldsDoNotOverwrite()
        {
            // Arrange
            var dto = new UpdateProductDto { Price = 50.00m }; // Name is null
            var product = new Product { Name = "KeepMe", Price = 10.00m };

            // Act
            _mapper.Map(dto, product);

            // Assert
            Assert.Equal("KeepMe", product.Name);
            Assert.Equal(50.00m, product.Price);
        }

        // ─── User → UserDto ────────────────────────────────────────────────────

        [Fact]
        public void Map_UserToUserDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = 5,
                Username = "alice",
                Email = "alice@example.com",
                FirstName = "Alice",
                LastName = "Smith",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now
            };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Equal(user.Id, dto.Id);
            Assert.Equal(user.Username, dto.Username);
            Assert.Equal(user.Email, dto.Email);
            Assert.Equal(user.FirstName, dto.FirstName);
            Assert.Equal(user.LastName, dto.LastName);
            Assert.Equal(user.Role, dto.Role);
            Assert.Equal(user.IsActive, dto.IsActive);
            Assert.Equal(user.CreatedAt, dto.CreatedAt);
        }

        [Fact]
        public void Map_UserToUserDto_NullableFieldsPreserved()
        {
            // Arrange
            var user = new User { Username = "minimal", Email = "m@e.com", FirstName = null, LastName = null };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        // ─── CreateUserDto → User ──────────────────────────────────────────────

        [Fact]
        public void Map_CreateUserDtoToUser_MapsAllProperties()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "P@ssw0rd",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            Assert.Equal(dto.Username, user.Username);
            Assert.Equal(dto.Email, user.Email);
            Assert.Equal(dto.FirstName, user.FirstName);
            Assert.Equal(dto.LastName, user.LastName);
        }

        [Fact]
        public void Map_CreateUserDtoToUser_PasswordHashIgnored()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "hashtest",
                Email = "hash@example.com",
                Password = "PlainPassword"
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert – PasswordHash should NOT be set from Password field
            Assert.NotEqual("PlainPassword", user.PasswordHash);
        }

        [Fact]
        public void Map_CreateUserDtoToUser_NullableFieldsPreserved()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "minimal",
                Email = "min@example.com",
                Password = "pass",
                FirstName = null,
                LastName = null
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            Assert.Null(user.FirstName);
            Assert.Null(user.LastName);
        }
    }
}
