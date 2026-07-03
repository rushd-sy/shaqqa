// Favorite.cs
using RealEstate.Domain.Common;
using RealEstate.Domain.User;

namespace RealEstate.Domain.Favorites;

public class Favorite : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PropertyId { get; set; }
    public RealEstate Property { get; set; } = null!;
}