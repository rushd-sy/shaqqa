using RealEstate.Domain.Common;
using RealEstate.Domain.Documents.Enum;
using RealEstate.Domain.DocumentTypes;

namespace RealEstate.Domain.Documents;

public class Document : AuditableEntity
{
    public Document(Guid id) : base(id) { }
    public Guid DocumentTypeId { get; set; }
    public int DocumentableId { get; set; }
    public DocumentableType DocumentableType { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public DocumentType DocumentType { get; set; } = null!;
}
