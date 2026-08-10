using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.BackgroundServices;

public class PhoneVerificationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PhoneVerificationCleanupService> _logger;

    public PhoneVerificationCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<PhoneVerificationCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var deletedCount = await context.Set<PhoneVerification>()
                        .Where(pv => pv.ExpiresAtUtc < DateTimeOffset.UtcNow.AddDays(-1) || pv.IsUsed)
                        .ExecuteDeleteAsync(stoppingToken);

                    _logger.LogInformation("Cleaned up {Count} expired or used OTP records.", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up expired OTPs.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}