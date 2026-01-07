namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Description = "Description for Product 1",
                    Price = 19.99m,
                    Stock = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Description = "Description for Product 2",
                    Price = 29.99m,
                    Stock = 50
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Description = "Description for Product 3",
                    Price = 39.99m,
                    Stock = 75
                }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = "hashed_password_here"
                },
                new User
                {
                    Id = 2,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_admin_password"
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
