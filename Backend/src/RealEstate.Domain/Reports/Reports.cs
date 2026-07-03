// Report.cs
using RealEstate.Domain.Common;
using RealEstate.Domain.RealEstate;

namespace RealEstate.Domain.Report;

public class Report : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public RealEstate Property { get; set; } = null!;

    public Guid ReportedByUserId { get; set; }
    public User ReportedByUser { get; set; } = null!;

    public ReportReason Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
}