using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.RecentSearches;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class SearchQueryConfiguration : IEntityTypeConfiguration<SearchQuery>
{
    public void Configure(EntityTypeBuilder<SearchQuery> builder)
    {
        builder.ToTable("SearchQueries");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.PublicId).IsRequired();
        builder.HasIndex(s => s.PublicId).IsUnique();

        builder.Property(s => s.Query).IsRequired().HasMaxLength(255);
        builder.Property(s => s.SearchedAt).IsRequired();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.UserId, s.Query }).IsUnique();
    }
}