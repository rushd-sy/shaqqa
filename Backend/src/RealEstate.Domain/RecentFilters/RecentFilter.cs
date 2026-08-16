using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.RecentFilters;

public class RecentFilter : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public string FiltersJson { get; set; } = string.Empty;
    public string FiltersHash { get; set; } = string.Empty;
    public DateTimeOffset SavedAt { get; set; }

    public User User { get; set; } = null!;
}