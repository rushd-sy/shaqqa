using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Companies;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users.Notifications;
using RealEstate.Domain.Users.Enums;
using RealEstate.Domain.Users.BrokerRequests;
using RealEstate.Domain.VerificationRequests;
using RealEstate.Domain.AdvertisementViews;

namespace RealEstate.Domain.Users;

public class User : AuditableEntity
{
    public User(Guid id) : base(id) { }
    public Guid? CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; } = UserRole.Customer;

    public Company? Company { get; set; }
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<VerificationRequest> ProcessedVerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<AdvertisementView> AdvertisementViews { get; set; } = new List<AdvertisementView>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<BrokerRequest> BrokerRequests { get; set; } = new List<BrokerRequest>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}