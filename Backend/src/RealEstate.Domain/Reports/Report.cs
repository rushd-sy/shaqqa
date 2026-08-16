using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Reports.Enums;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Reports;

public class Report : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public int AdvertisementId { get; set; }
    public ReportReason Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
}