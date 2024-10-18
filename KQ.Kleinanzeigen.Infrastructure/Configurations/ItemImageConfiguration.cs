using KQ.Kleinanzeigen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KQ.Kleinanzeigen.Infrastructure.Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Item).WithMany(p => p.Images).HasForeignKey(p => p.ItemId);
    }
}
