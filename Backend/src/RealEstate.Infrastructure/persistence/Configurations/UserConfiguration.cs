using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.Enums;
using RealEstate.Infrastructure.Identity;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("UserProfiles");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedNever();
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(255);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).HasDefaultValue(UserRole.Customer).HasSentinel((UserRole)0);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<User>(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}