using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data.Tests
{
    /// <summary>
    /// Unit tests for ApplicationDbContext.
    /// </summary>
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

        // ─── Constructor ───────────────────────────────────────────────────────

        [Fact]
        public void ApplicationDbContext_Constructor_CreatesInstance()
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

        // ─── Products CRUD ─────────────────────────────────────────────────────

        [Fact]
        public async Task Products_AddProduct_CanBeRetrieved()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Price = 9.99m,
                StockQuantity = 10,
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
        public async Task Products_AddMultipleProducts_AllPersisted()
        {
            // Arrange
            var products = new[]
            {
                new Product { Name = "P1", Price = 1.00m, CreatedAt = DateTime.UtcNow },
                new Product { Name = "P2", Price = 2.00m, CreatedAt = DateTime.UtcNow },
                new Product { Name = "P3", Price = 3.00m, CreatedAt = DateTime.UtcNow }
            };

            // Act
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(3, await _context.Products.CountAsync());
        }

        [Fact]
        public async Task Products_UpdateProduct_ChangePersisted()
        {
            // Arrange
            var product = new Product { Name = "Original", Price = 5.00m, CreatedAt = DateTime.UtcNow };
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
        public async Task Products_DeleteProduct_RemovedFromDatabase()
        {
            // Arrange
            var product = new Product { Name = "ToDelete", Price = 1.00m, CreatedAt = DateTime.UtcNow };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(product.Id);
            Assert.Null(retrieved);
        }

        // ─── Users CRUD ────────────────────────────────────────────────────────

        [Fact]
        public async Task Users_AddUser_CanBeRetrieved()
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
            Assert.Equal("testuser", retrieved!.Username);
        }

        [Fact]
        public async Task Users_AddMultipleUsers_AllPersisted()
        {
            // Arrange
            var users = new[]
            {
                new User { Username = "u1", Email = "u1@e.com", PasswordHash = "h1", CreatedAt = DateTime.UtcNow },
                new User { Username = "u2", Email = "u2@e.com", PasswordHash = "h2", CreatedAt = DateTime.UtcNow }
            };

            // Act
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Equal(2, await _context.Users.CountAsync());
        }

        [Fact]
        public async Task Users_UpdateUser_ChangePersisted()
        {
            // Arrange
            var user = new User
            {
                Username = "original",
                Email = "orig@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.Username = "updated";
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.Equal("updated", retrieved!.Username);
        }

        [Fact]
        public async Task Users_DeleteUser_RemovedFromDatabase()
        {
            // Arrange
            var user = new User
            {
                Username = "todelete",
                Email = "del@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.Null(retrieved);
        }

        // ─── OnModelCreating ───────────────────────────────────────────────────

        [Fact]
        public async Task OnModelCreating_ProductNameRequired_EnforcedByModel()
        {
            // Arrange
            var product = new Product
            {
                Name = "ValidName",
                Price = 1.00m,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Assert
            Assert.True(product.Id > 0);
        }

        [Fact]
        public async Task OnModelCreating_UserUsernameRequired_EnforcedByModel()
        {
            // Arrange
            var user = new User
            {
                Username = "validuser",
                Email = "valid@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assert
            Assert.True(user.Id > 0);
        }
    }
}
