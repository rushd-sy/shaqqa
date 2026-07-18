using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstate.Domain.PhoneVerifications
{
    public interface IPhoneVerificationRepository
    {
        Task AddAsync(PhoneVerification phoneVerification, CancellationToken cancellationToken= default);
        Task<PhoneVerification?> GetLatestByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    }
}
