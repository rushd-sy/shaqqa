using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Favorites;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).ValueGeneratedOnAdd();
        builder.Property(f => f.PublicId).IsRequired();
        builder.HasIndex(f => f.PublicId).IsUnique();

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Advertisement)
            .WithMany(a => a.Favorites)
            .HasForeignKey(f => f.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.UserId, f.AdvertisementId }).IsUnique();
    }
}