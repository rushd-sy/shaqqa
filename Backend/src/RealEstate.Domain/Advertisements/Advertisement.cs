using RealEstate.Domain.AdvertisementViews;
using RealEstate.Domain.Common;
using RealEstate.Domain.Favorites;
using RealEstate.Domain.Properties;
using RealEstate.Domain.Reports;
using RealEstate.Domain.Users;
using RealEstate.Domain.VerificationRequests;
using RealEstate.Domain.Advertisements.Enums;

namespace RealEstate.Domain.Advertisements;

public class Advertisement : AuditablePublicEntity
{
    public Guid UserId { get; set; }
    public int PropertyId { get; set; }
    public int? SupersededAdvertisementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContractType ContractType { get; set; }
    public string ContactInfo { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal AreaValue { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
    public AdvertisementStatus Status { get; set; }

    public User User { get; set; } = null!;
    public Property Property { get; set; } = null!;
    public Advertisement? SupersededAdvertisement { get; set; }

    public ICollection<VerificationRequest> VerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<AdvertisementView> AdvertisementViews { get; set; } = new List<AdvertisementView>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Media> Media { get; set; } = new List<Media>();
}