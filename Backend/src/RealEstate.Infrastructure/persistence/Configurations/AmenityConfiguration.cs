using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Amenities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("Amenities");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.IconUrl).HasMaxLength(500);
        builder.Property(a => a.Description).HasMaxLength(500);
    }
}
