using RealEstate.Domain.Common;
using RealEstate.Domain.Documents;


namespace RealEstate.Domain.DocumentTypes;

public class DocumentType : AuditableEntity
{
    public DocumentType(Guid id) : base(id) { }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AllowedExtensions { get; set; } = string.Empty;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
