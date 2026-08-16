using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.AdvertisementViews;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class AdvertisementViewConfiguration : IEntityTypeConfiguration<AdvertisementView>
{
    public void Configure(EntityTypeBuilder<AdvertisementView> builder)
    {
        builder.ToTable("AdvertisementViews");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(v => v.ViewedAt).IsRequired();

        builder.HasOne(v => v.User)
            .WithMany(u => u.AdvertisementViews)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Advertisement)
            .WithMany(a => a.AdvertisementViews)
            .HasForeignKey(v => v.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.UserId, v.AdvertisementId }).IsUnique();
    }
}