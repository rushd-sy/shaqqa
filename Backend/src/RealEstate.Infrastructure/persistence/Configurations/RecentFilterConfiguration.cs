using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.RecentFilters;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class RecentFilterConfiguration : IEntityTypeConfiguration<RecentFilter>
{
    public void Configure(EntityTypeBuilder<RecentFilter> builder)
    {
        builder.ToTable("RecentFilters");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        builder.Property(r => r.PublicId).IsRequired();
        builder.HasIndex(r => r.PublicId).IsUnique();

        builder.Property(r => r.FiltersJson).IsRequired();
        builder.Property(r => r.FiltersHash).IsRequired().HasMaxLength(64);
        builder.Property(r => r.SavedAt).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.UserId, r.FiltersHash }).IsUnique();
    }
}