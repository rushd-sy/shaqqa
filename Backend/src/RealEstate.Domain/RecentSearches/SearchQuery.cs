using RealEstate.Domain.Common;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.RecentSearches;

public class SearchQuery : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public string Query { get; set; } = string.Empty;
    public DateTimeOffset SearchedAt { get; set; }

    public User User { get; set; } = null!;
}