using Microsoft.AspNetCore.Identity;

namespace RealEstate.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid> { }

    public class ApplicationRole : IdentityRole<Guid> { }
}
