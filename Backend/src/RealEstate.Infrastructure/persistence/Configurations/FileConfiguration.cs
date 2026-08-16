using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.StoredFiles;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("Files");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).ValueGeneratedOnAdd();
        builder.Property(f => f.PublicId).IsRequired();
        builder.HasIndex(f => f.PublicId).IsUnique();

        builder.Property(f => f.FileName).IsRequired().HasMaxLength(255);
        builder.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(f => f.SizeBytes).IsRequired();
        builder.Property(f => f.StoredPath).IsRequired().HasMaxLength(500);
        builder.HasIndex(f => f.StoredPath).IsUnique();
    }
}