using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.TrustMetrics;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class TrustMetricConfiguration : IEntityTypeConfiguration<TrustMetric>
{
    public void Configure(EntityTypeBuilder<TrustMetric> builder)
    {
        builder.ToTable("TrustMetrics");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.ProfessionalPostsRatio).HasPrecision(3, 2);
        builder.Property(t => t.PostsProfScore).HasPrecision(5, 2);
        builder.Property(t => t.MonthlyPostsScore).HasPrecision(5, 2);
        builder.Property(t => t.ActivityScore).HasPrecision(5, 2);
        builder.Property(t => t.TrustScore).HasPrecision(5, 2);

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.AgentId, t.CalculatedAt });
    }
}