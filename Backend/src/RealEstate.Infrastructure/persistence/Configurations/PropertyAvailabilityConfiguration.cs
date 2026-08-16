using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Bookings;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class PropertyAvailabilityConfiguration : IEntityTypeConfiguration<PropertyAvailability>
{
    public void Configure(EntityTypeBuilder<PropertyAvailability> builder)
    {
        builder.ToTable("Property_Availability");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.PublicId).IsRequired();
        builder.HasIndex(p => p.PublicId).IsUnique();

        builder.Property(p => p.StartTime).IsRequired();
        builder.Property(p => p.EndTime).IsRequired();
        builder.Property(p => p.IsBooked).IsRequired();

        builder.HasOne(p => p.Property)
            .WithMany()
            .HasForeignKey(p => p.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.PropertyId, p.StartTime });
    }
}