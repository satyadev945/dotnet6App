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

        public void Dispose() => _context.Dispose();

        // ── Constructor ───────────────────────────────────────────────────────

        [Fact]
        public void ApplicationDbContext_Constructor_CreatesContext()
        {
            Assert.NotNull(_context);
        }

        // ── DbSets ────────────────────────────────────────────────────────────

        [Fact]
        public void ApplicationDbContext_Products_DbSetIsNotNull()
        {
            Assert.NotNull(_context.Products);
        }

        [Fact]
        public void ApplicationDbContext_Users_DbSetIsNotNull()
        {
            Assert.NotNull(_context.Users);
        }

        // ── Product CRUD ──────────────────────────────────────────────────────

        [Fact]
        public async Task ApplicationDbContext_AddProduct_PersistsToDatabase()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 10m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var saved = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(saved);
            Assert.Equal("Test Product", saved!.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_UpdateProduct_SavesChanges()
        {
            var product = new Product
            {
                Name = "Original",
                Price = 10m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product.Name = "Updated";
            await _context.SaveChangesAsync();

            var updated = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Updated", updated!.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_DeleteProduct_RemovesFromDatabase()
        {
            var product = new Product
            {
                Name = "To Delete",
                Price = 10m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var deleted = await _context.Products.FindAsync(product.Id);
            Assert.Null(deleted);
        }

        // ── User CRUD ─────────────────────────────────────────────────────────

        [Fact]
        public async Task ApplicationDbContext_AddUser_PersistsToDatabase()
        {
            var user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var saved = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("testuser", saved!.Username);
        }

        [Fact]
        public async Task ApplicationDbContext_UpdateUser_SavesChanges()
        {
            var user = new User
            {
                Username = "updateuser",
                Email = "update@test.com",
                PasswordHash = "hash",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.FirstName = "Updated";
            await _context.SaveChangesAsync();

            var updated = await _context.Users.FindAsync(user.Id);
            Assert.Equal("Updated", updated!.FirstName);
        }

        [Fact]
        public async Task ApplicationDbContext_DeleteUser_RemovesFromDatabase()
        {
            var user = new User
            {
                Username = "deleteuser",
                Email = "delete@test.com",
                PasswordHash = "hash",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            var deleted = await _context.Users.FindAsync(user.Id);
            Assert.Null(deleted);
        }

        // ── Multiple Entities ─────────────────────────────────────────────────

        [Fact]
        public async Task ApplicationDbContext_MultipleProducts_AllPersisted()
        {
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
            await _context.SaveChangesAsync();

            var count = await _context.Products.CountAsync();
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task ApplicationDbContext_MultipleUsers_AllPersisted()
        {
            for (int i = 1; i <= 3; i++)
            {
                _context.Users.Add(new User
                {
                    Username = $"user{i}",
                    Email = $"user{i}@test.com",
                    PasswordHash = "hash",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();

            var count = await _context.Users.CountAsync();
            Assert.Equal(3, count);
        }
    }

    public class DataSeederTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public DataSeederTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        public void Dispose() => _context.Dispose();

        // ── SeedAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsProducts()
        {
            await DataSeeder.SeedAsync(_context);

            var count = await _context.Products.CountAsync();
            Assert.True(count > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsUsers()
        {
            await DataSeeder.SeedAsync(_context);

            var count = await _context.Users.CountAsync();
            Assert.True(count > 0);
        }

        [Fact]
        public async Task SeedAsync_CalledTwice_DoesNotDuplicateProducts()
        {
            await DataSeeder.SeedAsync(_context);
            var countAfterFirst = await _context.Products.CountAsync();

            await DataSeeder.SeedAsync(_context);
            var countAfterSecond = await _context.Products.CountAsync();

            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        [Fact]
        public async Task SeedAsync_CalledTwice_DoesNotDuplicateUsers()
        {
            await DataSeeder.SeedAsync(_context);
            var countAfterFirst = await _context.Users.CountAsync();

            await DataSeeder.SeedAsync(_context);
            var countAfterSecond = await _context.Users.CountAsync();

            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        [Fact]
        public async Task SeedAsync_SeedsAdminUser()
        {
            await DataSeeder.SeedAsync(_context);

            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "admin");

            Assert.NotNull(admin);
            Assert.Equal("Admin", admin!.Role);
        }

        [Fact]
        public async Task SeedAsync_SeedsRegularUser()
        {
            await DataSeeder.SeedAsync(_context);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "john.doe");

            Assert.NotNull(user);
            Assert.Equal("User", user!.Role);
        }

        [Fact]
        public async Task SeedAsync_AllSeededProductsAreActive()
        {
            await DataSeeder.SeedAsync(_context);

            var inactiveCount = await _context.Products
                .CountAsync(p => !p.IsActive);

            Assert.Equal(0, inactiveCount);
        }

        [Fact]
        public async Task SeedAsync_AllSeededUsersAreActive()
        {
            await DataSeeder.SeedAsync(_context);

            var inactiveCount = await _context.Users
                .CountAsync(u => !u.IsActive);

            Assert.Equal(0, inactiveCount);
        }

        [Fact]
        public async Task SeedAsync_SeededProductsHaveValidPrices()
        {
            await DataSeeder.SeedAsync(_context);

            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Price > 0));
        }

        [Fact]
        public async Task SeedAsync_SeededProductsHaveNames()
        {
            await DataSeeder.SeedAsync(_context);

            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.NotEmpty(p.Name));
        }

        [Fact]
        public async Task SeedAsync_SeededUsersHavePasswordHashes()
        {
            await DataSeeder.SeedAsync(_context);

            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.NotEmpty(u.PasswordHash));
        }
    }
}
