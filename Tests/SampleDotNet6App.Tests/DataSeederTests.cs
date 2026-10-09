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
    /// Unit tests for DataSeeder.
    /// </summary>
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

        // ─── SeedAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var count = await _context.Products.CountAsync();
            Assert.True(count > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsUsers()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var count = await _context.Users.CountAsync();
            Assert.True(count > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsThreeProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            Assert.Equal(3, await _context.Products.CountAsync());
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsTwoUsers()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            Assert.Equal(2, await _context.Users.CountAsync());
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsAdminUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.Equal("Admin", admin!.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsTestUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var testUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(testUser);
            Assert.Equal("User", testUser!.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_ProductsHaveCorrectCategories()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var categories = await _context.Products.Select(p => p.Category).ToListAsync();
            Assert.Contains("Electronics", categories);
            Assert.Contains("Books", categories);
            Assert.Contains("Clothing", categories);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AllProductsAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var allActive = await _context.Products.AllAsync(p => p.IsActive);
            Assert.True(allActive);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AllUsersAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var allActive = await _context.Users.AllAsync(u => u.IsActive);
            Assert.True(allActive);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeeded_DoesNotDuplicateProducts()
        {
            // Arrange – seed once
            await DataSeeder.SeedAsync(_context);
            var countAfterFirst = await _context.Products.CountAsync();

            // Act – seed again
            await DataSeeder.SeedAsync(_context);
            var countAfterSecond = await _context.Products.CountAsync();

            // Assert
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeeded_DoesNotDuplicateUsers()
        {
            // Arrange – seed once
            await DataSeeder.SeedAsync(_context);
            var countAfterFirst = await _context.Users.CountAsync();

            // Act – seed again
            await DataSeeder.SeedAsync(_context);
            var countAfterSecond = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_ProductsHavePositivePrices()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var allPositive = await _context.Products.AllAsync(p => p.Price > 0);
            Assert.True(allPositive);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_UsersHaveHashedPasswords()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.PasswordHash)));
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AdminHasCorrectEmail()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.Equal("admin@example.com", admin!.Email);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_ProductsHavePositiveStock()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var allPositive = await _context.Products.AllAsync(p => p.StockQuantity > 0);
            Assert.True(allPositive);
        }
    }
}
