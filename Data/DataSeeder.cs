namespace SampleDotNet6App.Data;

public static class DataSeeder
{
    public static Task SeedAsync(ApplicationDbContext context)
    {
        // Data seeding logic can be added here
        return Task.CompletedTask;
    }
}
