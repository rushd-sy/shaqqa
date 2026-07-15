using System.Security.Claims;

using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;

namespace RealEstate.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

}