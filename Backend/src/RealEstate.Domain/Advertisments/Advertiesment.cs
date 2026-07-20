using RealEstate.Domain.Common;
using RealEstate.Domain.Enquiries;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Histories;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users;
using RealEstate.Domain.VerificationRequests;

namespace RealEstate.Domain.Advertisements;

public class Advertisement : AuditableEntity
{
    public Advertisement(Guid id) : base(id) { }
    public Guid UserId { get; set; }
    public Guid PropertyId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public bool IsListed { get; set; }

    public User User { get; set; } = null!;
    public Property Property { get; set; } = null!;

    public ICollection<VerificationRequest> VerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<History> HistoryEntries { get; set; } = new List<History>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
}
