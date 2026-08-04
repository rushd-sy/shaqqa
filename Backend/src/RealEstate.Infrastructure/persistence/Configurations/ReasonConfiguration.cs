using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Reasons;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class ReasonConfiguration : IEntityTypeConfiguration<Reason>
{
    public void Configure(EntityTypeBuilder<Reason> builder)
    {
        builder.ToTable("Reasons");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.Code).IsUnique();

        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);
    }
}
