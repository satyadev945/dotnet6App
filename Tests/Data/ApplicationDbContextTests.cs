using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Data
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
        public void ApplicationDbContext_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public void ApplicationDbContext_HasProductsDbSet()
        {
            // Assert
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void ApplicationDbContext_HasUsersDbSet()
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
            Assert.Equal("Test Product", retrieved.Name);
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("testuser", retrieved.Username);
        }

        [Fact]
        public async Task ApplicationDbContext_CanUpdateProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Original",
                Price = 10.00m,
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
                Name = "To Delete",
                Price = 5.00m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var productId = product.Id;

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(productId);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task ApplicationDbContext_CanQueryProductsByIsActive()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Active1", Price = 1m, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Active2", Price = 2m, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Inactive", Price = 3m, IsActive = false, CreatedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var activeProducts = await _context.Products.Where(p => p.IsActive).ToListAsync();

            // Assert
            Assert.Equal(2, activeProducts.Count);
        }

        [Fact]
        public async Task ApplicationDbContext_CanQueryUsersByIsActive()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "active1", Email = "a1@test.com", PasswordHash = "h", IsActive = true, CreatedAt = DateTime.UtcNow },
                new User { Username = "inactive1", Email = "i1@test.com", PasswordHash = "h", IsActive = false, CreatedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var activeUsers = await _context.Users.Where(u => u.IsActive).ToListAsync();

            // Assert
            Assert.Single(activeUsers);
        }
    }
}
