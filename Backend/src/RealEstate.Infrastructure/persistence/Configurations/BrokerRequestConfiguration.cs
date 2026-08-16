using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Users.BrokerRequests;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class BrokerRequestConfiguration : IEntityTypeConfiguration<BrokerRequest>
{
    public void Configure(EntityTypeBuilder<BrokerRequest> builder)
    {
        builder.ToTable("Broker_Request");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.PublicId).IsRequired();
        builder.HasIndex(b => b.PublicId).IsUnique();

        builder.Property(b => b.PriorExperience).HasDefaultValue(false);
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.RequestNotes).HasMaxLength(2000);

        builder.HasOne(b => b.User)
            .WithMany(u => u.BrokerRequests)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Location)
            .WithMany(l => l.BrokerRequests)
            .HasForeignKey(b => b.IdLocation)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.ReviewedByUser)
            .WithMany()
            .HasForeignKey(b => b.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.UserId, b.Status });
    }
}