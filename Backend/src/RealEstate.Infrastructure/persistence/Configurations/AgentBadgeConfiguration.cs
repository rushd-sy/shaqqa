using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.AgentBadges;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class AgentBadgeConfiguration : IEntityTypeConfiguration<AgentBadge>
{
    public void Configure(EntityTypeBuilder<AgentBadge> builder)
    {
        builder.ToTable("AgentBadges");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.PublicId).IsRequired();
        builder.HasIndex(b => b.PublicId).IsUnique();

        builder.Property(b => b.BadgeName).IsRequired().HasMaxLength(100).HasDefaultValue("Professional Agent");
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.GrantedAt).IsRequired();
        builder.Property(b => b.GrantedAtScore).HasPrecision(5, 2);
        builder.Property(b => b.RevokedAtScore).HasPrecision(5, 2);

        builder.HasOne(b => b.Agent)
            .WithMany()
            .HasForeignKey(b => b.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Grantor)
            .WithMany()
            .HasForeignKey(b => b.GrantedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Revoker)
            .WithMany()
            .HasForeignKey(b => b.RevokedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.AgentId, b.Status });
    }
}