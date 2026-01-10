namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new[]
            {
                new Product
                {
                    Name = "Laptop",
                    Description = "High performance laptop",
                    Price = 1299.99m,
                    Stock = 50,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Mouse",
                    Description = "Wireless mouse",
                    Price = 29.99m,
                    Stock = 200,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Keyboard",
                    Description = "Mechanical keyboard",
                    Price = 89.99m,
                    Stock = 100,
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here",
                    CreatedDate = DateTime.UtcNow
                },
                new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hashed_password_here",
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
