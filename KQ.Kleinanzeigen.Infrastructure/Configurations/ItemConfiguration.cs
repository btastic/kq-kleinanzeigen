using KQ.Kleinanzeigen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KQ.Kleinanzeigen.Infrastructure.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.Images).WithOne(k => k.Item).HasForeignKey(p => p.ItemId);

        builder.Property(p => p.Price).HasColumnType("decimal(18, 6)");
        builder.Property(p => p.ShippingCost).HasColumnType("decimal(18, 6)");

        builder.HasOne(p => p.Seller).WithMany(p => p.Items).HasForeignKey(p => p.SellerId);
    }
}
