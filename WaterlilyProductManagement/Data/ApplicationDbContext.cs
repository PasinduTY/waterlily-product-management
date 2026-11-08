using Microsoft.EntityFrameworkCore;
using WaterlilyProductManagement.Models;
using WaterlilyProductManagement.Models.Reports;

namespace WaterlilyProductManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the database precision for product prices.
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // Report result models are keyless because they map
        // to stored procedure results, not database tables.
        modelBuilder.Entity<CategoryAveragePrice>().HasNoKey();
        modelBuilder.Entity<HighestStockValueCategory>().HasNoKey();
    }
}
