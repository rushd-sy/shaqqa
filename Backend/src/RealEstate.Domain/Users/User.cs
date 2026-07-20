using RealEstate.Domain.Advertisements;
using RealEstate.Domain.Common;
using RealEstate.Domain.Companies;
using RealEstate.Domain.Enquiries;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Histories;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users.Notifications;
using RealEstate.Domain.VerificationRequests;

namespace RealEstate.Domain.Users;

public class User : AuditableEntity
{
    public User(Guid id) : base(id) { }
    public Guid? CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Company? Company { get; set; }
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<VerificationRequest> ProcessedVerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<History> HistoryEntries { get; set; } = new List<History>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
