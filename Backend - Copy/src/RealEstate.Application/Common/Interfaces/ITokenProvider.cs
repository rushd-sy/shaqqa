using System.Security.Claims;

using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;

namespace RealEstate.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

    Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    Task<Result<TokenResponse>> RefreshTokenAsync(string rawRefreshToken, CancellationToken ct = default);

}