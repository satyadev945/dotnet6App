using System;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Mappings.Tests
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MappingProfile_ConfigurationIsValid()
        {
            // Verify the mapper can be created without exceptions
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
            var mapper = config.CreateMapper();
            Assert.NotNull(mapper);
        }

        // ─── Product → ProductDto ─────────────────────────────────────────────────

        [Fact]
        public void Map_ProductToProductDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-end laptop",
                Price = 999.99m,
                Category = "Electronics",
                StockQuantity = 10,
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
            Assert.Equal(product.Category, dto.Category);
            Assert.Equal(product.StockQuantity, dto.StockQuantity);
            Assert.Equal(product.IsActive, dto.IsActive);
            Assert.Equal(product.CreatedAt, dto.CreatedAt);
            Assert.Equal(product.UpdatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void Map_ProductToProductDto_NullableFieldsPreserved()
        {
            // Arrange
            var product = new Product
            {
                Id = 2,
                Name = "Widget",
                Description = null,
                Category = null,
                UpdatedAt = null
            };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
            Assert.Null(dto.UpdatedAt);
        }

        // ─── CreateProductDto → Product ───────────────────────────────────────────

        [Fact]
        public void Map_CreateProductDtoToProduct_MapsAllProperties()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Desc",
                Price = 49.99m,
                Category = "NewCat",
                StockQuantity = 5
            };

            // Act
            var product = _mapper.Map<Product>(createDto);

            // Assert
            Assert.Equal(createDto.Name, product.Name);
            Assert.Equal(createDto.Description, product.Description);
            Assert.Equal(createDto.Price, product.Price);
            Assert.Equal(createDto.Category, product.Category);
            Assert.Equal(createDto.StockQuantity, product.StockQuantity);
        }

        // ─── UpdateProductDto → Product ───────────────────────────────────────────

        [Fact]
        public void Map_UpdateProductDtoToProduct_MapsNonNullProperties()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Price = 99.99m
            };
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Name",
                Price = 10m,
                Category = "OldCat"
            };

            // Act
            _mapper.Map(updateDto, existingProduct);

            // Assert
            Assert.Equal("Updated Name", existingProduct.Name);
            Assert.Equal(99.99m, existingProduct.Price);
        }

        // ─── User → UserDto ───────────────────────────────────────────────────────

        [Fact]
        public void Map_UserToUserDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = 1,
                Username = "alice",
                Email = "alice@example.com",
                PasswordHash = "hash",
                FirstName = "Alice",
                LastName = "Smith",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now,
                LastLoginAt = now
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
            Assert.Equal(user.LastLoginAt, dto.LastLoginAt);
        }

        [Fact]
        public void Map_UserToUserDto_NullableFieldsPreserved()
        {
            // Arrange
            var user = new User
            {
                Id = 2,
                Username = "bob",
                Email = "bob@example.com",
                PasswordHash = "hash",
                FirstName = null,
                LastName = null,
                LastLoginAt = null
            };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
            Assert.Null(dto.LastLoginAt);
        }

        // ─── RegisterUserDto → User ───────────────────────────────────────────────

        [Fact]
        public void Map_RegisterUserDtoToUser_MapsAllProperties()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "charlie",
                Email = "charlie@example.com",
                Password = "Pass123!",
                FirstName = "Charlie",
                LastName = "Brown"
            };

            // Act
            var user = _mapper.Map<User>(registerDto);

            // Assert
            Assert.Equal(registerDto.Username, user.Username);
            Assert.Equal(registerDto.Email, user.Email);
            Assert.Equal(registerDto.FirstName, user.FirstName);
            Assert.Equal(registerDto.LastName, user.LastName);
        }

        [Fact]
        public void Map_IEnumerableProductToProductDto_MapsCollection()
        {
            // Arrange
            var products = new[]
            {
                new Product { Id = 1, Name = "A", Price = 1m },
                new Product { Id = 2, Name = "B", Price = 2m }
            };

            // Act
            var dtos = _mapper.Map<System.Collections.Generic.IEnumerable<ProductDto>>(products);

            // Assert
            Assert.Equal(2, System.Linq.Enumerable.Count(dtos));
        }
    }
}
