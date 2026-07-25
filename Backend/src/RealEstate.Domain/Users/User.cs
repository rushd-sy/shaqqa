using RealEstate.Domain.Common;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users.Notifications;

namespace RealEstate.Domain.Users;

public class User : AuditableEntity
{
    public User(Guid id) : base(id)
    {
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
