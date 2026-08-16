using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.AgentBadges;

public class AgentBadge : AuditablePublicEntity
{
    public Guid AgentId { get; set; }
    public Guid GrantedBy { get; set; }
    public Guid? RevokedBy { get; set; }
    public string BadgeName { get; set; } = "Professional Agent";
    public AgentBadgeStatus Status { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public decimal GrantedAtScore { get; set; }
    public decimal RevokedAtScore { get; set; }

    public User Agent { get; set; } = null!;
    public User Grantor { get; set; } = null!;
    public User? Revoker { get; set; }
}