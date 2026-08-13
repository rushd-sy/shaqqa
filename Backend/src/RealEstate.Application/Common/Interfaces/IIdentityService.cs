using RealEstate.Application.DTOs;
using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;

namespace RealEstate.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<bool>> SendOtpAsync(SendOtpDto dto, CancellationToken cancellationToken = default);
    Task<Result<TokenResponse>> RegisterWithOtpAsync(RegisterWithOtpDto dto, CancellationToken cancellationToken = default);
    Task<Result<TokenResponse>> LoginWithOtpAsync(LoginWithOtpDto dto,CancellationToken cancellationToken = default);
}