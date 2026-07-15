using System.Security.Claims;

namespace RealEstate.Application.Identity.DTOs;

public sealed record AppUserDto(string UserId, string Email, IList<string> Roles, IList<Claim> Claims);