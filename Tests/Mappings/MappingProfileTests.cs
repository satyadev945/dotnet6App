using System;
using AutoMapper;
using Xunit;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace SampleDotNet6App.Tests.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            // Use DI to create mapper (same as production code)
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            var provider = services.BuildServiceProvider();
            _mapper = provider.GetRequiredService<IMapper>();
        }

        [Fact]
        public void MappingProfile_MapperIsCreated_Successfully()
        {
            // Verify the mapper was created successfully
            Assert.NotNull(_mapper);
        }

        // ---- Product -> ProductDto ----
        [Fact]
        public void Map_Product_To_ProductDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "A description",
                Price = 49.99m,
                Category = "Electronics",
                StockQuantity = 25,
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
        public void Map_Product_To_ProductDto_WithNullDescription_MapsCorrectly()
        {
            // Arrange
            var product = new Product
            {
                Id = 2,
                Name = "No Desc",
                Description = null,
                Price = 10.00m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Null(dto.Description);
        }

        // ---- CreateProductDto -> Product ----
        [Fact]
        public void Map_CreateProductDto_To_Product_MapsAllProperties()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 29.99m,
                Category = "Gadgets",
                StockQuantity = 50
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

        // ---- User -> UserDto ----
        [Fact]
        public void Map_User_To_UserDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "shouldnotbemapped",
                FirstName = "Test",
                LastName = "User",
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
        public void Map_User_To_UserDto_WithNullOptionalFields_MapsCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = 2,
                Username = "noname",
                Email = "noname@example.com",
                PasswordHash = "hash",
                FirstName = null,
                LastName = null,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Null(dto.FirstName);
            Assert.Null(dto.LastName);
        }

        // ---- RegisterUserDto -> User ----
        [Fact]
        public void Map_RegisterUserDto_To_User_MapsAllProperties()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "Pass123!",
                FirstName = "New",
                LastName = "User"
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
        public void Map_RegisterUserDto_To_User_IgnoresPasswordHash()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "secureuser",
                Email = "secure@example.com",
                Password = "ShouldNotBeHashed"
            };

            // Act
            var user = _mapper.Map<User>(registerDto);

            // Assert - PasswordHash should be ignored (empty/default)
            Assert.Equal(string.Empty, user.PasswordHash);
        }

        // ---- UpdateProductDto -> Product (conditional mapping) ----
        [Fact]
        public void Map_UpdateProductDto_To_Product_WithValues_OverwritesExisting()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Original Name",
                Price = 100.00m
            };

            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Price = 200.00m
            };

            // Act
            _mapper.Map(updateDto, existingProduct);

            // Assert
            Assert.Equal("Updated Name", existingProduct.Name);
            Assert.Equal(200.00m, existingProduct.Price);
        }

        [Fact]
        public void Map_UpdateProductDto_To_Product_WithNullValues_MappingCompletes()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Original Name",
                Price = 100.00m,
                Category = "Original Category"
            };

            var updateDto = new UpdateProductDto
            {
                Name = null,
                Price = null,
                Category = null
            };

            // Act - mapping should complete without exception
            _mapper.Map(updateDto, existingProduct);

            // Assert - product still exists and Id is preserved
            Assert.NotNull(existingProduct);
            Assert.Equal(1, existingProduct.Id);
        }
    }
}
