using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.DocumentTypes;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("DocumentTypes");
            builder.HasKey(dt => dt.Id);

            builder.Property(dt => dt.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(dt => dt.Code).IsUnique();

            builder.Property(dt => dt.Name).IsRequired().HasMaxLength(150);
            builder.Property(dt => dt.AllowedExtensions).HasMaxLength(200);
        }
    }
}
