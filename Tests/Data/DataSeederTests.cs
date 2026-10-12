using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Tests.Data
{
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

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── SeedAsync ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsUsers()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.NotEmpty(products);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsAdminUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.Equal("Admin", admin.Role);
            Assert.Equal("admin@example.com", admin.Email);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsManagerUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var manager = await _context.Users.FirstOrDefaultAsync(u => u.Username == "jane.smith");
            Assert.NotNull(manager);
            Assert.Equal("Manager", manager.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsRegularUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "john.doe");
            Assert.NotNull(user);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AllUsersAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsExpectedNumberOfProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.Equal(5, products.Count);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsLaptopProduct()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var laptop = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop Pro 15");
            Assert.NotNull(laptop);
            Assert.Equal("Electronics", laptop.Category);
            Assert.Equal(1299.99m, laptop.Price);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AllProductsAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task SeedAsync_AlreadySeededDatabase_DoesNotDuplicateUsers()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var initialCount = await _context.Users.CountAsync();

            // Act
            await DataSeeder.SeedAsync(_context);
            var finalCount = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(initialCount, finalCount);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeededDatabase_DoesNotDuplicateProducts()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var initialCount = await _context.Products.CountAsync();

            // Act
            await DataSeeder.SeedAsync(_context);
            var finalCount = await _context.Products.CountAsync();

            // Assert
            Assert.Equal(initialCount, finalCount);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_UsersHaveHashedPasswords()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.NotEmpty(u.PasswordHash));
            Assert.All(users, u => Assert.NotEqual("Admin123!", u.PasswordHash));
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_ProductsHavePositivePrices()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Price > 0));
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_ProductsHavePositiveStock()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.StockQuantity > 0));
        }
    }
}
