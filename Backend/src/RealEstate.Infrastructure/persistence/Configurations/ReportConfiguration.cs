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

        builder.HasOne(r => r.Advertisement)
            .WithMany(a => a.Reports)
            .HasForeignKey(r => r.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reason)
            .WithMany(rs => rs.Reports)
            .HasForeignKey(r => r.ReasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReportedByUser)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
