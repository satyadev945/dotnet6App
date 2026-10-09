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
        public async Task SeedAsync_EmptyDatabase_SeedsProducts()
        {
            // Arrange - empty database

            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var productCount = await _context.Products.CountAsync();
            Assert.True(productCount > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsUsers()
        {
            // Arrange - empty database

            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var userCount = await _context.Users.CountAsync();
            Assert.True(userCount > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsThreeProducts()
        {
            // Arrange - empty database

            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var productCount = await _context.Products.CountAsync();
            Assert.Equal(3, productCount);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsTwoUsers()
        {
            // Arrange - empty database

            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var userCount = await _context.Users.CountAsync();
            Assert.Equal(2, userCount);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeeded_DoesNotDuplicateProducts()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var countAfterFirstSeed = await _context.Products.CountAsync();

            // Act
            await DataSeeder.SeedAsync(_context);
            var countAfterSecondSeed = await _context.Products.CountAsync();

            // Assert
            Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeeded_DoesNotDuplicateUsers()
        {
            // Arrange
            await DataSeeder.SeedAsync(_context);
            var countAfterFirstSeed = await _context.Users.CountAsync();

            // Act
            await DataSeeder.SeedAsync(_context);
            var countAfterSecondSeed = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
        }

        [Fact]
        public async Task SeedAsync_SeededProducts_HaveValidNames()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.False(string.IsNullOrEmpty(p.Name)));
        }

        [Fact]
        public async Task SeedAsync_SeededProducts_HavePositivePrices()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Price > 0));
        }

        [Fact]
        public async Task SeedAsync_SeededProducts_AreActive()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task SeedAsync_SeededUsers_HaveValidUsernames()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.Username)));
        }

        [Fact]
        public async Task SeedAsync_SeededUsers_HaveValidEmails()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.Contains("@", u.Email));
        }

        [Fact]
        public async Task SeedAsync_SeededUsers_AreActive()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task SeedAsync_SeededUsers_HaveHashedPasswords()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.PasswordHash)));
        }

        [Fact]
        public async Task SeedAsync_SeededUsers_ContainsAdminUser()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(adminUser);
            Assert.Equal("Admin", adminUser.Role);
        }

        [Fact]
        public async Task SeedAsync_SeededProducts_ContainElectronicsCategory()
        {
            // Arrange & Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var electronicsProduct = await _context.Products.FirstOrDefaultAsync(p => p.Category == "Electronics");
            Assert.NotNull(electronicsProduct);
        }
    }
}
