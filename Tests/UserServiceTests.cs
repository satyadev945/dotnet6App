using Xunit;
using SampleDotNet6App.Services;
using SampleDotNet6App.Models;
using SampleDotNet6App.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SampleDotNet6App.Tests
{
    public class UserServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new UserService(context);

            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task CreateUser_ValidUser_AddsUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new UserService(context);

            var newUser = new User { Username = "newuser", Email = "new@example.com" };

            // Act
            var result = await service.CreateUserAsync(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task GetUserByUsername_ValidUsername_ReturnsUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new UserService(context);

            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }
    }
}
