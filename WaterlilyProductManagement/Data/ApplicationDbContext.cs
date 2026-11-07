
using Microsoft.EntityFrameworkCore;
using WaterlilyProductManagement.Models;

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

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(p => p.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(p => p.Price)
                .HasPrecision(18, 2);
        });
    }
}

