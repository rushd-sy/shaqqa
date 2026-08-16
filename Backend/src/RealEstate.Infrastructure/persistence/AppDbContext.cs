using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Advertisements;
using RealEstate.Domain.AgentBadges;
using RealEstate.Domain.Amenities;
using RealEstate.Domain.AdvertisementViews;
using RealEstate.Domain.Bookings;
using RealEstate.Domain.Companies;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Locations;
using RealEstate.Domain.Properties;
using RealEstate.Domain.PropertyAmenities;
using RealEstate.Domain.RecentFilters;
using RealEstate.Domain.RecentSearches;
using RealEstate.Domain.Reasons;
using RealEstate.Domain.Reports;
using RealEstate.Domain.TrustMetrics;
using RealEstate.Domain.Users;
using RealEstate.Domain.Users.BrokerRequests;
using RealEstate.Domain.Users.Notifications;
using RealEstate.Domain.VerificationRequests;
using RealEstate.Infrastructure.Identity;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Domain.StoredFiles;
using RealEstate.Domain.Common;
using RealEstate.Infrastructure.Persistence.ValueGenerators;

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
        public DbSet<PhoneVerification> PhoneVerifications => Set<PhoneVerification>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
        public DbSet<Advertisement> Advertisements => Set<Advertisement>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<StoredFile> Files => Set<StoredFile>();
        public DbSet<BrokerRequest> BrokerRequests => Set<BrokerRequest>();
        public DbSet<TrustMetric> TrustMetrics => Set<TrustMetric>();
        public DbSet<AgentBadge> AgentBadges => Set<AgentBadge>();
        public DbSet<SearchQuery> SearchQueries => Set<SearchQuery>();
        public DbSet<RecentFilter> RecentFilters => Set<RecentFilter>();
        public DbSet<AdvertisementView> AdvertisementViews => Set<AdvertisementView>();
        public DbSet<BookingVisit> BookingVisits => Set<BookingVisit>();
        public DbSet<PropertyAvailability> PropertyAvailability => Set<PropertyAvailability>();
        public DbSet<Reason> Reasons => Set<Reason>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(t => typeof(PublicEntity).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>(nameof(PublicEntity.PublicId))
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<SqlServerUuidV7Generator>();
            }
        }


    }
}