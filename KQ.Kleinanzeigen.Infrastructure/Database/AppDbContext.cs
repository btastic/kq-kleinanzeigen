using KQ.Kleinanzeigen.Domain.Entities;
using KQ.Kleinanzeigen.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace KQ.Kleinanzeigen.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<ItemImage> ItemImages { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ItemConfiguration());
        modelBuilder.ApplyConfiguration(new ItemImageConfiguration());
        modelBuilder.ApplyConfiguration(new SellerConfiguration());
    }
}
