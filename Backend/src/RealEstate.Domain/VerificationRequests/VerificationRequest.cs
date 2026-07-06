using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Users;
using RealEstate.Domain.VerificationRequests.Enums;

namespace RealEstate.Domain.VerificationRequests;

public class VerificationRequest : AuditableEntity
{
    public VerificationRequest(Guid id) : base(id)
    {
    }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public Guid? ReviewedByUserId { get; set; }
    public User? ReviewedByUser { get; set; }

    public VerificationStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? ReviewedAt { get; set; }
}