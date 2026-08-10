using RealEstate.Domain.Common;
namespace RealEstate.Domain.PhoneVerifications
{
    public class PhoneVerification : AuditableEntity
    {
        public PhoneVerification(Guid id ) :base(id)
        { 
        }  

        public string PhoneNumber { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public bool IsExpired(DateTimeOffset now) => now > ExpiresAtUtc;
        public bool IsUsed { get; set; } = false;
        public int FailedAttempts { get; set; } = 0;
    }
}
