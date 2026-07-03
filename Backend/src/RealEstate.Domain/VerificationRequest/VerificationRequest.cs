// VerificationRequest.cs
using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class VerificationRequest : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public Guid? ReviewedByUserId { get; set; }
    public User? ReviewedByUser { get; set; }

    public VerificationStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? ReviewedAt { get; set; }
}