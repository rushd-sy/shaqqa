using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Locations;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public static readonly Guid SyriaPublicId = Guid.Parse("11111111-1111-7111-8111-111111111111");

    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.PublicId).IsRequired();
        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.Code).IsRequired().HasMaxLength(10);
        builder.HasIndex(c => c.Code).IsUnique();

        builder.HasData(new Country(-1, SyriaPublicId)
        {
            Name = "Syria",
            Code = "SY",
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });
    }
}