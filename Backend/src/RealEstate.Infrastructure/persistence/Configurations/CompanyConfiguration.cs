using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Companies;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.PublicId).IsRequired();
        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(c => c.PhoneNumber).IsUnique();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(c => c.Location)
            .WithMany(l => l.Companies)
            .HasForeignKey(c => c.IdLocation)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Employees)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .HasPrincipalKey(c => c.PublicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}