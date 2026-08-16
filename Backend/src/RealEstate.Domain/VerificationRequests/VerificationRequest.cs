using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Users;
using RealEstate.Domain.VerificationRequests.Enums;

namespace RealEstate.Domain.VerificationRequests
{
    public class VerificationRequest : AuditablePublicEntity
    {
        public int AdvertisementId { get; set; }
        public Guid UserId { get; set; }
        public VerificationRequestType RequestType { get; set; }
        public VerificationPriority Priority { get; set; }
        public VerificationStatus Status { get; set; }
        public Guid? ReviewedBy { get; set; }
        public string? AdminNote { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }

        public Advertisement Advertisement { get; set; } = null!;
        public User User { get; set; } = null!;
        public User? ReviewedByUser { get; set; }
    }
}