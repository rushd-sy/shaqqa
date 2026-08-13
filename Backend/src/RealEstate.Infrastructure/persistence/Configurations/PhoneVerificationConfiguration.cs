using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.PhoneVerifications;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class PhoneVerificationConfiguration : IEntityTypeConfiguration<PhoneVerification>
{
    public void Configure(EntityTypeBuilder<PhoneVerification> builder)
    {
        builder.ToTable("PhoneVerifications");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(pv => pv.VerificationCode)
            .IsRequired()
            .HasMaxLength(256);
        builder.HasIndex(pv => new { pv.PhoneNumber, pv.IsUsed, pv.ExpiresAtUtc });
    }
}