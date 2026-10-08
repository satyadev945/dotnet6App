using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Data.Tests
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
        public async Task SeedAsync_EmptyDatabase_SeedsUsers()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var userCount = await _context.Users.CountAsync();
            Assert.True(userCount > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var productCount = await _context.Products.CountAsync();
            Assert.True(productCount > 0);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_CreatesAdminUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.Equal("Admin", admin!.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_CreatesManagerUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var manager = await _context.Users.FirstOrDefaultAsync(u => u.Username == "jane.smith");
            Assert.NotNull(manager);
            Assert.Equal("Manager", manager!.Role);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_CreatesRegularUser()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "john.doe");
            Assert.NotNull(user);
            Assert.Equal("User", user!.Role);
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
        public async Task SeedAsync_EmptyDatabase_AllProductsAreActive()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_CreatesLaptopProduct()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var laptop = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop Pro 15");
            Assert.NotNull(laptop);
            Assert.Equal("Electronics", laptop!.Category);
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
        public async Task SeedAsync_PasswordsAreHashed()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert – passwords should not be stored in plain text
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Assert.NotNull(admin);
            Assert.NotEqual("Admin123!", admin!.PasswordHash);
            Assert.NotEmpty(admin.PasswordHash);
        }

        [Fact]
        public async Task SeedAsync_ProductsHavePositivePrices()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Price > 0));
        }

        [Fact]
        public async Task SeedAsync_ProductsHavePositiveStock()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.StockQuantity > 0));
        }
    }
}
