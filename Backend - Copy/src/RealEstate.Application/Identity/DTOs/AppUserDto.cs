
namespace RealEstate.Application.Identity.DTOs;

public sealed record AppUserDto(Guid UserId, string Email, IList<string> Roles);