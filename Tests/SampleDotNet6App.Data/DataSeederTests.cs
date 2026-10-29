using System;
using System.Linq;
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

        // ── SeedAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsThreeProducts()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var count = await _context.Products.CountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsCorrectProductNames()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var names = await _context.Products.Select(p => p.Name).ToListAsync();
            Assert.Contains("Sample Product 1", names);
            Assert.Contains("Sample Product 2", names);
            Assert.Contains("Sample Product 3", names);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsCorrectPrices()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            Assert.Equal(9.99m, products[0].Price);
            Assert.Equal(19.99m, products[1].Price);
            Assert.Equal(29.99m, products[2].Price);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsCorrectStockValues()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            Assert.Equal(100, products[0].Stock);
            Assert.Equal(50, products[1].Stock);
            Assert.Equal(25, products[2].Stock);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_SeedsCorrectDescriptions()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            Assert.Equal("Description 1", products[0].Description);
            Assert.Equal("Description 2", products[1].Description);
            Assert.Equal("Description 3", products[2].Description);
        }

        [Fact]
        public async Task SeedAsync_AlreadySeeded_DoesNotAddMoreProducts()
        {
            // Arrange - seed once
            await DataSeeder.SeedAsync(_context);

            // Act - seed again
            await DataSeeder.SeedAsync(_context);

            // Assert - still only 3 products
            var count = await _context.Products.CountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task SeedAsync_DatabaseWithExistingProducts_SkipsSeed()
        {
            // Arrange - add a product manually
            _context.Products.Add(new Product { Name = "Existing", Price = 1m, Stock = 1 });
            await _context.SaveChangesAsync();

            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert - only the manually added product remains
            var count = await _context.Products.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task SeedAsync_EmptyDatabase_AllProductsHavePositiveIds()
        {
            // Act
            await DataSeeder.SeedAsync(_context);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.All(products, p => Assert.True(p.Id > 0));
        }

        [Fact]
        public async Task SeedAsync_CalledMultipleTimes_IdempotentBehavior()
        {
            // Act
            await DataSeeder.SeedAsync(_context);
            await DataSeeder.SeedAsync(_context);
            await DataSeeder.SeedAsync(_context);

            // Assert
            var count = await _context.Products.CountAsync();
            Assert.Equal(3, count);
        }
    }
}
