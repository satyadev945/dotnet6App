using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data.Tests
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void ApplicationDbContext_Constructor_CreatesContext()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public void ApplicationDbContext_Products_DbSetNotNull()
        {
            // Assert
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void ApplicationDbContext_Users_DbSetNotNull()
        {
            // Assert
            Assert.NotNull(_context.Users);
        }

        [Fact]
        public async Task ApplicationDbContext_CanAddAndRetrieveProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Price = 9.99m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Test Product", retrieved!.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_CanAddAndRetrieveUser()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("testuser", retrieved!.Username);
        }

        [Fact]
        public async Task ApplicationDbContext_CanUpdateProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Original",
                Price = 10m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated";
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Updated", retrieved!.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_CanDeleteProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "ToDelete",
                Price = 5m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var id = product.Id;

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(id);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task ApplicationDbContext_MultipleProducts_AllPersisted()
        {
            // Arrange
            for (int i = 1; i <= 5; i++)
            {
                _context.Products.Add(new Product
                {
                    Name = $"Product {i}",
                    Price = i * 10m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Act
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.Products.CountAsync();
            Assert.Equal(5, count);
        }
    }
}
