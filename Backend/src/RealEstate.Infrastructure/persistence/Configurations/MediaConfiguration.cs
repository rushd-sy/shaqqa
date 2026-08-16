using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Advertisements;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.Property(m => m.PublicId).IsRequired();
        builder.HasIndex(m => m.PublicId).IsUnique();

        builder.Property(m => m.IsCover).IsRequired();
        builder.Property(m => m.DisplayOrder).IsRequired();

        builder.HasOne(m => m.Advertisement)
            .WithMany(a => a.Media)
            .HasForeignKey(m => m.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.File)
            .WithMany(f => f.Media)
            .HasForeignKey(m => m.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.AdvertisementId, m.DisplayOrder });
    }
}