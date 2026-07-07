using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.Notifications;
using RealEstate.Domain.VerificationRequests;
using RealEstate.Infrastructure.Identity;

namespace RealEstate.Infrastructure.persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> AppUsers => Set<User>();
        //public DbSet<Property> Properties => Set<Property>();
        //public DbSet<Report> Reports => Set<Report>();
        //public DbSet<Favorite> Favorites => Set<Favorite>();
        //public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
        //public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Property>();
            modelBuilder.Ignore<Report>();
            modelBuilder.Ignore<Favorite>();
            modelBuilder.Ignore<VerificationRequest>();
            modelBuilder.Ignore<Notification>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


    }
}
