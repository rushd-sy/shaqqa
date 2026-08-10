using RealEstate.Application.Common.Interfaces;

namespace RealEstate.Infrastructure.Services;

public class FakeSmsService : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[Fake SMS] Sent to {phoneNumber}: {message}");
        return Task.CompletedTask;
    }
}