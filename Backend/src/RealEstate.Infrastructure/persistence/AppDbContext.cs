using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Amenities;
using RealEstate.Domain.Companies;
using RealEstate.Domain.Documents;
using RealEstate.Domain.DocumentTypes;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Histories;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Properties;
using RealEstate.Domain.PropertyAmenities;
using RealEstate.Domain.Reasons;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.Notifications;
using RealEstate.Domain.VerificationRequests;
using RealEstate.Infrastructure.Identity;

namespace RealEstate.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> UserProfiles => Set<User>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
        public DbSet<Advertisement> Advertisements => Set<Advertisement>();
        public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<History> Histories => Set<History>();
        public DbSet<Reason> Reasons => Set<Reason>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        }


    }
}
