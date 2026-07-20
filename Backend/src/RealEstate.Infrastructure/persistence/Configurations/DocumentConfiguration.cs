using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Documents;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DocumentableType).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => new { d.DocumentableType, d.DocumentableId });

            builder.Property(d => d.Filename).IsRequired().HasMaxLength(255);
            builder.Property(d => d.Url).IsRequired().HasMaxLength(1000);

            builder.HasOne(d => d.DocumentType)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
