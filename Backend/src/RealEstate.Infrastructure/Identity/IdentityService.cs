using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Domain.Users;
using RealEstate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IPhoneVerificationRepository _phoneVerificationRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly ITokenProvider _tokenProvider;
    private readonly AppDbContext _context;

    public IdentityService(
        IPhoneVerificationRepository phoneVerificationRepository,
        UserManager<ApplicationUser> userManager,
        //ITokenProvider tokenProvider,
        AppDbContext context)
    {
        _phoneVerificationRepository = phoneVerificationRepository;
        _userManager = userManager;
        //_tokenProvider = tokenProvider;
        _context = context;
    }

    public async Task<Result<bool>> SendOtpAsync(SendOtpDto dto, CancellationToken cancellationToken = default)
    {
        var userExists = await _userManager.Users
        .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber, cancellationToken);

        if (userExists)
        {
            return Error.Conflict("User.AlreadyExists", "Phone number is already registered.");
        }

        var activeVerifications = await _context.PhoneVerifications
    .Where(pv => pv.PhoneNumber == dto.PhoneNumber && !pv.IsUsed)
    .ToListAsync(cancellationToken);

        foreach (var activeVerification in activeVerifications)
        {
            activeVerification.IsUsed = true;
        }
        var randomOtpCode = Random.Shared.Next(100000, 999999).ToString();

        var verification = new PhoneVerification(Guid.NewGuid())
        {
            PhoneNumber = dto.PhoneNumber,
            VerificationCode = randomOtpCode,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _phoneVerificationRepository.AddAsync(verification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Result<TokenResponse>> RegisterWithOtpAsync(RegisterWithOtpDto dto, CancellationToken cancellationToken = default)
    {
        var userExists = await _userManager.Users
        .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber, cancellationToken);

        if (userExists)
        {
            return Error.Conflict("User.AlreadyExists", "Phone number is already registered.");
        }

        var verification = await _context.PhoneVerifications
            .Where(pv => pv.PhoneNumber == dto.PhoneNumber
                      && pv.VerificationCode == dto.VerificationCode
                      && !pv.IsUsed
                      && pv.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(pv => pv.ExpiresAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
        if (verification == null || verification.VerificationCode != dto.VerificationCode)
        {
            return Error.Validation("OTP.InvalidCode");
        }
        if (verification.IsUsed || verification.IsExpired)
        {
            return Error.Validation("OTP.Expired");
        }
        verification.IsUsed = true;
        var appUser = new ApplicationUser
        {
            UserName = dto.PhoneNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = $"{dto.PhoneNumber}@realestate.com",
            PhoneNumberConfirmed = true
        };
        var createResult = await _userManager.CreateAsync(appUser);

        if (!createResult.Succeeded)
        {
            var errorDescription = createResult.Errors.FirstOrDefault()?.Description ?? "General failure.";

            return Error.Failure(errorDescription);
        }
        var domainUser = new User(appUser.Id)
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = true
        };
        _context.Set<User>().Add(domainUser);

        var fakeToken = new TokenResponse
        {
            AccessToken = $"fake-jwt-token-{Guid.NewGuid()}",
            RefreshToken = $"fake-refresh-token-{Guid.NewGuid()}"
        };

        await _context.SaveChangesAsync(cancellationToken);
        return fakeToken;

        // var tokenResponse = await _tokenProvider.GenerateTokenAsync(user);
        // return Result.Success(tokenResponse);
    }
}