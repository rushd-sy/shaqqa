using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.TrustMetrics;

public class TrustMetric : AuditableInternalEntity
{
    public Guid AgentId { get; set; }
    public bool HasPhotoVerifiedByAdmin { get; set; }
    public decimal ProfessionalPostsRatio { get; set; }
    public int PostsThisMonth { get; set; }
    public int ActiveDaysLast30 { get; set; }
    public decimal PostsProfScore { get; set; }
    public decimal MonthlyPostsScore { get; set; }
    public decimal ActivityScore { get; set; }
    public decimal TrustScore { get; set; }
    public DateTimeOffset CalculatedAt { get; set; }

    public User Agent { get; set; } = null!;
}