using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Locations;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).ValueGeneratedOnAdd();
        builder.Property(l => l.PublicId).IsRequired();
        builder.HasIndex(l => l.PublicId).IsUnique();

        builder.Property(l => l.PlaceId).HasMaxLength(255);

        builder.HasIndex(l => l.PlaceId)
            .IsUnique()
            .HasFilter("[PlaceId] IS NOT NULL");

        builder.Property(l => l.Address).IsRequired().HasMaxLength(500);
        builder.Property(l => l.Latitude).HasPrecision(9, 6);
        builder.Property(l => l.Longitude).HasPrecision(9, 6);

        builder.HasOne(l => l.City)
            .WithMany(c => c.Locations)
            .HasForeignKey(l => l.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Country)
            .WithMany(c => c.Locations)
            .HasForeignKey(l => l.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}