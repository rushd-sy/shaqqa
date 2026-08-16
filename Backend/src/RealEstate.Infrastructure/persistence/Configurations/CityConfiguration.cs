using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Locations;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.PublicId).IsRequired();
        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

        builder.HasIndex(c => new { c.CountryId, c.Name }).IsUnique();

        builder.HasOne(c => c.Country)
            .WithMany(c => c.Cities)
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SyrianGovernorates);
    }

    private static (int Id, string Name)[] GovernorateRows =>
    [
        (-1, "Damascus"),
        (-2, "Aleppo"),
        (-3, "Homs"),
        (-4, "Hama"),
        (-5, "Latakia"),
        (-6, "Tartus"),
        (-7, "Idlib"),
        (-8, "Raqqa"),
        (-9, "Deir ez-Zor"),
        (-10, "Al-Hasakah"),
        (-11, "Daraa"),
        (-12, "As-Suwayda"),
        (-13, "Quneitra"),
        (-14, "Rif Dimashq")
    ];

    private static City[] SyrianGovernorates =>
        GovernorateRows.Select((g, i) => new City(g.Id, Guid.Parse($"22222222-2222-7222-8222-{i + 1:D12}"))
        {
            CountryId = -1,
            Name = g.Name,
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        }).ToArray();
}