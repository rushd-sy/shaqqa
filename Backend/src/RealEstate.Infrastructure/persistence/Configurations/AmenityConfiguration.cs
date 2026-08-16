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

        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Name).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Description).HasMaxLength(1000);

        builder.HasMany(a => a.PropertyAmenities)
            .WithOne(pa => pa.Amenity)
            .HasForeignKey(pa => pa.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}