using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.PhoneVerifications;

namespace RealEstate.Infrastructure.Persistence.Repositories;

public class PhoneVerificationRepository : IPhoneVerificationRepository
{
    private readonly AppDbContext _context;
    public PhoneVerificationRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken = default)
    {
        await _context.PhoneVerifications.AddAsync(phoneVerification, cancellationToken);
    }
    public async Task<PhoneVerification?> GetLatestByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.PhoneVerifications
            .Where(pv => pv.PhoneNumber == phoneNumber)
            .OrderByDescending(pv => pv.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}