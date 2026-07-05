using RealEstate.Domain.Common;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users.Enums;
using RealEstate.Domain.Users.Notifications;

namespace RealEstate.Domain.Users;

public class User : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}