using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Histories;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class HistoryConfiguration : IEntityTypeConfiguration<History>
{
    public void Configure(EntityTypeBuilder<History> builder)
    {
        builder.ToTable("Histories");
        builder.HasKey(h => h.Id);

        builder.HasOne(h => h.User)
            .WithMany(u => u.HistoryEntries)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Advertisement)
            .WithMany(a => a.HistoryEntries)
            .HasForeignKey(h => h.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
