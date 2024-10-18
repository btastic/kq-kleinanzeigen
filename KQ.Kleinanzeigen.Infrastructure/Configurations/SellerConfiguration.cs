using KQ.Kleinanzeigen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KQ.Kleinanzeigen.Infrastructure.Configurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Rating).IsRequired(false);
        builder.Property(p => p.Friendliness).IsRequired(false);
        builder.Property(p => p.Reliability).IsRequired(false);

        builder.HasMany(p => p.Items).WithOne(k => k.Seller).HasForeignKey(p => p.SellerId);
    }
}
