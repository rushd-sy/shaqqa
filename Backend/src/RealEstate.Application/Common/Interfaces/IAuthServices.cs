using RealEstate.Application.Identity.DTOs;

namespace RealEstate.Application.Common.Interfaces;

public interface IAuthServices
{
    Task RevokeTokenAsync(string rawRefreshToken, Guid user ,  CancellationToken ct = default);

}