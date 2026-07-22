using RealEstate.Domain.PhoneVerifications;
namespace RealEstate.Application.Common.Interfaces
{
    public interface IPhoneVerificationRepository
    {
        Task AddAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken= default);
        Task<PhoneVerification?> GetLatestByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    }
}
