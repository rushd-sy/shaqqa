using RealEstate.Domain.Common;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Favorites;

public class Favorite : AuditableEntity
{
    public Favorite(Guid id) : base(id)
    {
    }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}