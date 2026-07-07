using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.persistence;

namespace RealEstate.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
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
