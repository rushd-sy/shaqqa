using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Bookings;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class BookingVisitConfiguration : IEntityTypeConfiguration<BookingVisit>
{
    public void Configure(EntityTypeBuilder<BookingVisit> builder)
    {
        builder.ToTable("Booking_Visit");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.PublicId).IsRequired();
        builder.HasIndex(b => b.PublicId).IsUnique();

        builder.Property(b => b.AppointmentDatetime).IsRequired();
        builder.Property(b => b.Status).IsRequired();

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Property)
            .WithMany()
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Availability)
            .WithMany(a => a.Bookings)
            .HasForeignKey(b => b.AvailabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.UserId, b.Status });
    }
}