using Microsoft.EntityFrameworkCore;

namespace SampleDotNet6App.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSet properties here
    // public DbSet<YourEntity> YourEntities { get; set; }
}
