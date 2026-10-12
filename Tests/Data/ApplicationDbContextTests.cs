using System;
using System.Collections.Generic;
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

        // ─── Constructor ───────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_ValidOptions_CreatesContext()
        {
            // Assert
            Assert.NotNull(_context);
        }

        // ─── Products DbSet ────────────────────────────────────────────────────────

        [Fact]
        public void Products_DbSet_IsNotNull()
        {
            // Assert
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public async Task Products_AddProduct_PersistsToDatabase()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 9.99m,
                Category = "Test",
                StockQuantity = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
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
        public async Task Products_UpdateProduct_PersistsChanges()
        {
            // Arrange
            var product = new Product { Name = "Original", Description = "desc", Price = 1m, Category = "Cat", StockQuantity = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
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
        public async Task Products_DeleteProduct_RemovesFromDatabase()
        {
            // Arrange
            var product = new Product { Name = "ToDelete", Description = "desc", Price = 1m, Category = "Cat", StockQuantity = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
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

        // ─── Users DbSet ───────────────────────────────────────────────────────────

        [Fact]
        public void Users_DbSet_IsNotNull()
        {
            // Assert
            Assert.NotNull(_context.Users);
        }

        [Fact]
        public async Task Users_AddUser_PersistsToDatabase()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hash",
                FirstName = "Test",
                LastName = "User",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
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
        public async Task Users_UpdateUser_PersistsChanges()
        {
            // Arrange
            var user = new User { Username = "original", Email = "original@example.com", PasswordHash = "hash", FirstName = "F", LastName = "L", Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.FirstName = "Updated";
            await _context.SaveChangesAsync();

            // Assert
            var updated = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.FirstName);
        }

        [Fact]
        public async Task Users_DeleteUser_RemovesFromDatabase()
        {
            // Arrange
            var user = new User { Username = "todelete", Email = "todelete@example.com", PasswordHash = "hash", FirstName = "F", LastName = "L", Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var id = user.Id;

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var deleted = await _context.Users.FindAsync(id);
            Assert.Null(deleted);
        }

        // ─── SaveChangesAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task SaveChangesAsync_MultipleEntities_PersistsAll()
        {
            // Arrange
            var product1 = new Product { Name = "P1", Description = "d", Price = 1m, Category = "C", StockQuantity = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
            var product2 = new Product { Name = "P2", Description = "d", Price = 2m, Category = "C", StockQuantity = 2, IsActive = true, CreatedAt = DateTime.UtcNow };

            // Act
            _context.Products.AddRange(product1, product2);
            var count = await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(2, count);
        }
    }
}
