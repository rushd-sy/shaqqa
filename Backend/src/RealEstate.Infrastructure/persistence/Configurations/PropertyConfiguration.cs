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

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.PropertyType).IsRequired();
            builder.Property(p => p.LegalStatus).IsRequired();
            builder.Property(p => p.Description).IsRequired();

            builder.HasOne(p => p.Location)
                .WithOne(l => l.Property)
                .HasForeignKey<Property>(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.LocationId).IsUnique();

            builder.HasMany(p => p.Advertisements)
                .WithOne(a => a.Property)
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PropertyAmenities)
                .WithOne(pa => pa.Property)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.FloorNumber);
        }
    }
}