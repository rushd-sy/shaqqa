using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Properties;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Properties");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UnitNumber).HasMaxLength(50);
            builder.Property(p => p.PropertyType).IsRequired();
            builder.Property(p => p.LegalStatus).IsRequired();
            builder.Property(p => p.Area).IsRequired();

            builder.HasOne(p => p.Location)
                .WithMany(p => p.Properties)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Advertisements)
                .WithOne(p => p.Property)
                .HasForeignKey(p => p.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PropertyAmenities)
                .WithOne(pa => pa.Property)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.LocationId, p.FloorNumber, p.UnitNumber });

        }
    }
}
