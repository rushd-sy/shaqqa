using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Persistence.Repositories;

namespace RealEstate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPhoneVerificationRepository, PhoneVerificationRepository>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}