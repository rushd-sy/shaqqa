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

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(c => c.PocUser)
            .WithMany()
            .HasForeignKey(c => c.PocUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Employees)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
