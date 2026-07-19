using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Identity;

namespace RealEstate.Infrastructure.persistence.Configurations;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(500);
            builder.HasIndex(x=>x.TokenHash).IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(u=>u.RefreshTokens)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
}