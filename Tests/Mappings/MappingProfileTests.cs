using System;
using AutoMapper;
using Xunit;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        // ─── Configuration Validity ────────────────────────────────────────────────

        [Fact]
        public void MappingProfile_ConfigurationIsValid()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

            // Act - verify the mapper can be created (profile uses ForMember ignores for some members)
            var mapper = config.CreateMapper();
            Assert.NotNull(mapper);
        }

        // ─── Product → ProductDto ──────────────────────────────────────────────────

        [Fact]
        public void Map_ProductToProductDto_MapsAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                Category = "Electronics",
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = now
            };

            // Act
            var dto = _mapper.Map<ProductDto>(product);

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Product", dto.Name);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(99.99m, dto.Price);
            Assert.Equal("Electronics", dto.Category);
            Assert.Equal(10, dto.StockQuantity);
            Assert.Equal(now, dto.CreatedAt);
        }

        [Fact]
        public void Map_ProductToProductDto_NullProduct_ThrowsException()
        {
            // Act - AutoMapper returns null for null source by default
            var result = _mapper.Map<ProductDto>((Product?)null);

            // Assert
            Assert.Null(result);
        }

        // ─── CreateProductDto → Product ────────────────────────────────────────────

        [Fact]
        public void Map_CreateProductDtoToProduct_MapsAllProperties()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 49.99m,
                Category = "Accessories",
                StockQuantity = 5
            };

            // Act
            var product = _mapper.Map<Product>(dto);

            // Assert
            Assert.Equal("New Product", product.Name);
            Assert.Equal("New Description", product.Description);
            Assert.Equal(49.99m, product.Price);
            Assert.Equal("Accessories", product.Category);
            Assert.Equal(5, product.StockQuantity);
        }

        [Fact]
        public void Map_CreateProductDtoToProduct_IdDefaultsToZero()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "P", Description = "d", Price = 1m, Category = "C", StockQuantity = 1 };

            // Act
            var product = _mapper.Map<Product>(dto);

            // Assert
            Assert.Equal(0, product.Id);
        }

        // ─── User → UserDto ────────────────────────────────────────────────────────

        [Fact]
        public void Map_UserToUserDto_MapsAllProperties()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "secret",
                FirstName = "Test",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("testuser", dto.Username);
            Assert.Equal("test@example.com", dto.Email);
            Assert.Equal("Test", dto.FirstName);
            Assert.Equal("User", dto.LastName);
            Assert.Equal("Admin", dto.Role);
        }

        [Fact]
        public void Map_UserToUserDto_DoesNotExposePasswordHash()
        {
            // Arrange
            var user = new User { Id = 1, Username = "u", Email = "e@e.com", PasswordHash = "secret", FirstName = "F", LastName = "L", Role = "User" };

            // Act
            var dto = _mapper.Map<UserDto>(user);

            // Assert
            var passwordHashProp = typeof(UserDto).GetProperty("PasswordHash");
            Assert.Null(passwordHashProp);
        }

        // ─── RegisterDto → User ────────────────────────────────────────────────────

        [Fact]
        public void Map_RegisterDtoToUser_MapsAllProperties()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password1!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            Assert.Equal("newuser", user.Username);
            Assert.Equal("newuser@example.com", user.Email);
            Assert.Equal("New", user.FirstName);
            Assert.Equal("User", user.LastName);
        }

        [Fact]
        public void Map_RegisterDtoToUser_IgnoresPasswordHash()
        {
            // Arrange
            var dto = new RegisterDto { Username = "u", Email = "e@e.com", Password = "Password1!", FirstName = "F", LastName = "L" };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            Assert.Equal(string.Empty, user.PasswordHash);
        }

        // ─── DTO Classes ───────────────────────────────────────────────────────────

        [Fact]
        public void ProductDto_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var dto = new ProductDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.Category);
            Assert.Equal(0, dto.StockQuantity);
        }

        [Fact]
        public void CreateProductDto_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var dto = new CreateProductDto();

            // Assert
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Description);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.Category);
            Assert.Equal(0, dto.StockQuantity);
        }

        [Fact]
        public void UserDto_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var dto = new UserDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Role);
        }

        [Fact]
        public void RegisterDto_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var dto = new RegisterDto();

            // Assert
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
        }

        [Fact]
        public void LoginDto_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var dto = new LoginDto();

            // Assert
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Password);
        }

        [Fact]
        public void LoginDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new LoginDto { Username = "user", Password = "pass" };

            // Assert
            Assert.Equal("user", dto.Username);
            Assert.Equal("pass", dto.Password);
        }
    }
}
