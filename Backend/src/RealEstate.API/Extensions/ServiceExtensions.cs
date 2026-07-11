using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            }
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
             {
                 options.Password.RequiredLength = 8;
                 options.Password.RequireNonAlphanumeric = true;
                 options.User.RequireUniqueEmail = true;
                 options.Lockout.MaxFailedAccessAttempts = 5;
             })
                 .AddEntityFrameworkStores<AppDbContext>()
                 .AddDefaultTokenProviders();
            return services;
        }

    }
}
