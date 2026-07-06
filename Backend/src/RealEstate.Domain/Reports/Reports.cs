using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports.Enums;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Reports;

public class Report : AuditableEntity
{
    public Report(Guid id) : base(id)
    {
    }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public Guid ReportedByUserId { get; set; }
    public User ReportedByUser { get; set; } = null!;

    public ReportReason Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
}