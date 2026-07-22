
namespace RealEstate.Application.Identity.DTOs;

public sealed record AppUserDto(Guid UserId, string Phone, IList<string> Roles);