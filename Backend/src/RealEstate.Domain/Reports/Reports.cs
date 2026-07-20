using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Reasons;
using RealEstate.Domain.Reports.Enums;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Reports;

public class Report : AuditableEntity
{
    public Report(Guid id) : base(id) { }

    public Guid AdvertisementId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public Guid ReasonId { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }


    public Advertisement Advertisement { get; set; } = null!;
    public Reason Reason { get; set; } = null!;
    public User ReportedByUser { get; set; } = null!;

}
