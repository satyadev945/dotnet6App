using System;
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
        public void ApplicationDbContext_Constructor_CreatesContext()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public void ApplicationDbContext_Products_DbSetIsNotNull()
        {
            // Assert
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void ApplicationDbContext_Users_DbSetIsNotNull()
        {
            // Assert
            Assert.NotNull(_context.Users);
        }

        [Fact]
        public async Task ApplicationDbContext_AddProduct_PersistsSuccessfully()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Price = 9.99m,
                StockQuantity = 10,
                IsActive = true
            };

            // Act
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(saved);
            Assert.Equal("Test Product", saved.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_AddUser_PersistsSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("testuser", saved.Username);
        }

        [Fact]
        public async Task ApplicationDbContext_UpdateProduct_PersistsChanges()
        {
            // Arrange
            var product = new Product { Name = "Original", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated";
            await _context.SaveChangesAsync();

            // Assert
            var updated = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_DeleteProduct_RemovesFromDatabase()
        {
            // Arrange
            var product = new Product { Name = "ToDelete", Price = 5m, IsActive = true };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var id = product.Id;

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var deleted = await _context.Products.FindAsync(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task ApplicationDbContext_MultipleProducts_AllPersisted()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "P1", Price = 1m, IsActive = true },
                new Product { Name = "P2", Price = 2m, IsActive = true },
                new Product { Name = "P3", Price = 3m, IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var count = await _context.Products.CountAsync();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task ApplicationDbContext_MultipleUsers_AllPersisted()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "u1", Email = "u1@example.com", PasswordHash = "h1", IsActive = true },
                new User { Username = "u2", Email = "u2@example.com", PasswordHash = "h2", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var count = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(2, count);
        }
    }
}
