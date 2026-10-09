using System;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Mappings
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
        public void MappingProfile_CanCreateMapper_WithoutException()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);

            // Act
            var mapper = config.CreateMapper();

            // Assert
            Assert.NotNull(mapper);
        }

        [Fact]
        public void Map_ProductToProductDto_MapsAllFields()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 19.99m,
                StockQuantity = 50,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = createdAt
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
        public void Map_CreateProductDtoToProduct_MapsAllFields()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 29.99m,
                StockQuantity = 100,
                Category = "Books"
            };

            // Act
            var product = _mapper.Map<Product>(createDto);

            // Assert
            Assert.Equal(createDto.Name, product.Name);
            Assert.Equal(createDto.Description, product.Description);
            Assert.Equal(createDto.Price, product.Price);
            Assert.Equal(createDto.StockQuantity, product.StockQuantity);
            Assert.Equal(createDto.Category, product.Category);
        }

        [Fact]
        public void Map_UpdateProductDtoToProduct_MapsNonNullFields()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Price = 39.99m
            };
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Name",
                Price = 10m,
                Description = "Old Desc",
                IsActive = true
            };

            // Act
            _mapper.Map(updateDto, existingProduct);

            // Assert
            Assert.Equal("Updated Name", existingProduct.Name);
            Assert.Equal(39.99m, existingProduct.Price);
        }

        [Fact]
        public void Map_UserToUserDto_MapsAllFields()
        {
            // Arrange
            var createdAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc);
            var user = new User
            {
                Id = 5,
                Username = "johndoe",
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true,
                CreatedAt = createdAt
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
        public void Map_CreateUserDtoToUser_MapsAllFieldsExceptPasswordHash()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Password123",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var user = _mapper.Map<User>(createDto);

            // Assert
            Assert.Equal(createDto.Username, user.Username);
            Assert.Equal(createDto.Email, user.Email);
            Assert.Equal(createDto.FirstName, user.FirstName);
            Assert.Equal(createDto.LastName, user.LastName);
            // PasswordHash should be ignored (not mapped from Password)
            Assert.Equal(string.Empty, user.PasswordHash);
        }

        [Fact]
        public void Map_ProductToProductDto_NullDescription_MapsToNull()
        {
            // Arrange
            var product = new Product
            {
                Id = 2,
                Name = "No Desc",
                Description = null,
                Price = 5m,
                IsActive = true
            };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Null(dto.Description);
        }

        [Fact]
        public void Map_UserToUserDto_NullOptionalFields_MapsToNull()
        {
            // Arrange
            var user = new User
            {
                Id = 3,
                Username = "noname",
                Email = "noname@example.com",
                PasswordHash = "hash",
                FirstName = null,
                LastName = null
            };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        [Fact]
        public void Map_UpdateProductDtoToProduct_NullFields_DoNotOverwriteExistingValues()
        {
            // Arrange
            var updateDto = new UpdateProductDto
            {
                Name = "New Name"
                // Description is null - should not overwrite existing
            };
            var existingProduct = new Product
            {
                Name = "Old Name",
                Description = "Keep This Description",
                Price = 10m
            };

            // Act
            _mapper.Map(updateDto, existingProduct);

            // Assert
            Assert.Equal("New Name", existingProduct.Name);
            Assert.Equal("Keep This Description", existingProduct.Description);
        }
    }
}
