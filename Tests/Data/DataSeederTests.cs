using System;
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

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_SeedsUsers()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var userCount = await _context.Users.CountAsync();
            Assert.True(userCount > 0);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_SeedsProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var productCount = await _context.Products.CountAsync();
            Assert.True(productCount > 0);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_CreatesAdminUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(adminUser);
            Assert.Equal("Admin", adminUser.Role);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_CreatesManagerUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var managerUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "jane.smith");
            Assert.NotNull(managerUser);
            Assert.Equal("Manager", managerUser.Role);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_CreatesRegularUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var regularUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "john.doe");
            Assert.NotNull(regularUser);
            Assert.Equal("User", regularUser.Role);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_AllUsersAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_AllProductsAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task SeedAsync_WithExistingUsers_DoesNotDuplicateUsers()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var initialCount = await _context.Users.CountAsync();

            // Act - seed again
            await DataSeeder.SeedAsync(_context);

            // Assert
            var finalCount = await _context.Users.CountAsync();
            Assert.Equal(initialCount, finalCount);
        }

        [Fact]
        public async Task SeedAsync_WithExistingProducts_DoesNotDuplicateProducts()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var initialCount = await _context.Products.CountAsync();

            // Act - seed again
            await DataSeeder.SeedAsync(_context);

            // Assert
            var finalCount = await _context.Products.CountAsync();
            Assert.Equal(initialCount, finalCount);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_SeedsLaptopProduct()
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
        public async Task SeedAsync_WithEmptyDatabase_SeedsWirelessMouseProduct()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var mouse = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Wireless Mouse");
            Assert.NotNull(mouse);
            Assert.Equal("Accessories", mouse.Category);
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_AllUsersHavePasswordHash()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.NotEmpty(u.PasswordHash));
        }

        [Fact]
        public async Task SeedAsync_WithEmptyDatabase_AllProductsHavePositivePrice()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Price > 0));
        }
    }
}
