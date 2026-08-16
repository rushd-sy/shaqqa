using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Advertisements;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.ToTable("Advertisements");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.PublicId).IsRequired();
        builder.HasIndex(a => a.PublicId).IsUnique();

        builder.Property(a => a.Title).IsRequired().HasMaxLength(255);
        builder.Property(a => a.ContactInfo).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Price).HasPrecision(14, 2);
        builder.Property(a => a.AreaValue).HasPrecision(14, 2);
        builder.Property(a => a.ContractType).IsRequired();
        builder.Property(a => a.Status).IsRequired();

        builder.HasOne(a => a.User)
            .WithMany(u => u.Advertisements)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Property)
            .WithMany(p => p.Advertisements)
            .HasForeignKey(a => a.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.SupersededAdvertisement)
            .WithMany()
            .HasForeignKey(a => a.SupersededAdvertisementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.PublishDate);
    }
}