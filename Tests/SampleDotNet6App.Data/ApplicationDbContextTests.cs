using System;
using System.Linq;
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

        // ── Constructor ───────────────────────────────────────────────────────

        [Fact]
        public void Constructor_ValidOptions_CreatesContext()
        {
            // Assert
            Assert.NotNull(_context);
        }

        // ── DbSets ────────────────────────────────────────────────────────────

        [Fact]
        public void Products_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void Users_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Users);
        }

        // ── Product CRUD ──────────────────────────────────────────────────────

        [Fact]
        public async Task Products_AddProduct_CanBeRetrieved()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Desc",
                Price = 9.99m,
                Stock = 10
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
            _context.Products.AddRange(
                new Product { Name = "P1", Price = 1m, Stock = 1 },
                new Product { Name = "P2", Price = 2m, Stock = 2 },
                new Product { Name = "P3", Price = 3m, Stock = 3 }
            );

            // Act
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.Products.CountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task Products_RemoveProduct_IsDeleted()
        {
            // Arrange
            var product = new Product { Name = "Delete Me", Price = 1m, Stock = 1 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(product.Id);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task Products_UpdateProduct_ChangesArePersisted()
        {
            // Arrange
            var product = new Product { Name = "Original", Price = 5m, Stock = 5 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated";
            product.Price = 10m;
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Updated", retrieved!.Name);
            Assert.Equal(10m, retrieved.Price);
        }

        // ── User CRUD ─────────────────────────────────────────────────────────

        [Fact]
        public async Task Users_AddUser_CanBeRetrieved()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash123"
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
            _context.Users.AddRange(
                new User { Username = "u1", Email = "u1@example.com", PasswordHash = "h1" },
                new User { Username = "u2", Email = "u2@example.com", PasswordHash = "h2" }
            );

            // Act
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.Users.CountAsync();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Users_RemoveUser_IsDeleted()
        {
            // Arrange
            var user = new User { Username = "delete_me", Email = "del@example.com", PasswordHash = "h" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task Users_UpdateUser_ChangesArePersisted()
        {
            // Arrange
            var user = new User { Username = "original", Email = "orig@example.com", PasswordHash = "h" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.Username = "updated_username";
            user.Email = "updated@example.com";
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.Users.FindAsync(user.Id);
            Assert.Equal("updated_username", retrieved!.Username);
            Assert.Equal("updated@example.com", retrieved.Email);
        }

        // ── Query tests ───────────────────────────────────────────────────────

        [Fact]
        public async Task Products_QueryByName_ReturnsMatchingProduct()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Apple", Price = 1m, Stock = 10 },
                new Product { Name = "Banana", Price = 2m, Stock = 20 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Apple");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Apple", result!.Name);
        }

        [Fact]
        public async Task Users_QueryByEmail_ReturnsMatchingUser()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Username = "alice", Email = "alice@example.com", PasswordHash = "h1" },
                new User { Username = "bob", Email = "bob@example.com", PasswordHash = "h2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == "bob@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("bob", result!.Username);
        }

        [Fact]
        public async Task Products_EmptyDatabase_CountIsZero()
        {
            // Act
            var count = await _context.Products.CountAsync();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task Users_EmptyDatabase_CountIsZero()
        {
            // Act
            var count = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(0, count);
        }
    }
}
