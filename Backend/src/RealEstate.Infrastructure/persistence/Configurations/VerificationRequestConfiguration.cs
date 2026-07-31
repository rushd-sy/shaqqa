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

        builder.Property(v => v.Status).IsRequired();

        builder.HasOne(v => v.Advertisement)
            .WithMany(a => a.VerificationRequests)
            .HasForeignKey(v => v.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.ReviewedByUser)
            .WithMany(u => u.ProcessedVerificationRequests)
            .HasForeignKey(v => v.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
