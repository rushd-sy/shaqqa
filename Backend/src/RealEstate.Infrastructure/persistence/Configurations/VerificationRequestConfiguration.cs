using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.VerificationRequests;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequest>
{
    public void Configure(EntityTypeBuilder<VerificationRequest> builder)
    {
        builder.ToTable("VerificationRequests");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(v => v.PublicId).IsRequired();
        builder.HasIndex(v => v.PublicId).IsUnique();

        builder.Property(v => v.RequestType).IsRequired();
        builder.Property(v => v.Priority).IsRequired();
        builder.Property(v => v.Status).IsRequired();
        builder.Property(v => v.AdminNote).HasMaxLength(1000);

        builder.HasOne(v => v.Advertisement)
            .WithMany(a => a.VerificationRequests)
            .HasForeignKey(v => v.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.ReviewedByUser)
            .WithMany(u => u.ProcessedVerificationRequests)
            .HasForeignKey(v => v.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.AdvertisementId, v.Status });
    }
}