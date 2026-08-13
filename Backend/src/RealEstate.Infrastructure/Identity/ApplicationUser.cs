using Microsoft.AspNetCore.Identity;

namespace RealEstate.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid> 
    {
        public bool IsActive { get; set; } = true;
        public Guid? CompanyId { get; set; }
    }
}
