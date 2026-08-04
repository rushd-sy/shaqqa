using RealEstate.Domain.Common;
using RealEstate.Domain.Reports;

namespace RealEstate.Domain.Reasons;

public class Reason : AuditableEntity
{
    public Reason(Guid id) : base(id) { }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
