using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Reports;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        builder.Property(r => r.PublicId).IsRequired();
        builder.HasIndex(r => r.PublicId).IsUnique();

        builder.Property(r => r.Reason).IsRequired();
        builder.Property(r => r.Status).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(2000);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Advertisement)
            .WithMany(a => a.Reports)
            .HasForeignKey(r => r.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.UserId, r.AdvertisementId }).IsUnique();
    }
}