using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Extensions;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Domain.Users;
using RealEstate.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace RealEstate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ISmsService _smsService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ITokenProvider tokenProvider,
        AppDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ISmsService smsService)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _context = context;
        _passwordHasher = passwordHasher;
        _smsService = smsService;
    }

    public async Task<Result<bool>> SendOtpAsync(SendOtpDto dto, CancellationToken cancellationToken = default)
    {
        var phoneNumber = dto.PhoneNumber.ToCanonicalE164();
        var userExists = await _userManager.Users
        .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        if (userExists)
        {
            return true;
        }
        var lastVerification = await _context.PhoneVerifications
        .Where(pv => pv.PhoneNumber == phoneNumber)
        .OrderByDescending(pv => pv.ExpiresAtUtc)
        .FirstOrDefaultAsync(cancellationToken);

        if (lastVerification != null && lastVerification.ExpiresAtUtc.AddMinutes(-4) > DateTimeOffset.UtcNow)
        {
            return Error.Failure("OTP.RateLimit", "Please wait a minute before requesting a new code.");
        }

        var activeVerifications = await _context.PhoneVerifications
        .Where(pv => pv.PhoneNumber == phoneNumber && !pv.IsUsed)
        .ToListAsync(cancellationToken);

        foreach (var activeVerification in activeVerifications)
        {
            activeVerification.IsUsed = true;
        }
        var randomOtpCode = GenerateSecureOtp();
        string hashedOtp = _passwordHasher.HashPassword(null!, randomOtpCode);

        var verification = new PhoneVerification(Guid.NewGuid())
        {
            PhoneNumber = phoneNumber,
            VerificationCode = hashedOtp,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _context.PhoneVerifications.AddAsync(verification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _smsService.SendSmsAsync(phoneNumber, $"Your OTP code is: {randomOtpCode}", cancellationToken);

        return true;
    }

    public async Task<Result<TokenResponse>> RegisterWithOtpAsync(RegisterWithOtpDto dto, CancellationToken cancellationToken = default)
    {
        var phoneNumber = dto.PhoneNumber.ToCanonicalE164();
        var userExists = await _userManager.Users
        .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        if (userExists)
        {
            return Error.Conflict("User.AlreadyExists", "Phone number is already registered.");
        }

        var latestVerification = await _context.PhoneVerifications
        .Where(pv => pv.PhoneNumber == phoneNumber)
        .OrderByDescending(pv => pv.CreatedAtUtc)
        .FirstOrDefaultAsync(cancellationToken);
        if(latestVerification == null)
    {
            return Error.Validation("OTP.NotFound", "No verification request found for this number.");
        }
        if (latestVerification.IsUsed)
        {
            return Error.Validation("OTP.AlreadyUsed", "This OTP has already been used.");
        }
        var now = DateTimeOffset.UtcNow;
        if (latestVerification.IsExpired(now))
        {
            return Error.Validation("OTP.Expired", "The verification code has expired.");
        }
        if (latestVerification.FailedAttempts >= 5)
        {
            latestVerification.IsUsed = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Error.Validation("OTP.MaxAttemptsExceeded", "Too many failed attempts. Please request a new code.");
        }
        var verificationResult = _passwordHasher.VerifyHashedPassword(null!, latestVerification.VerificationCode, dto.VerificationCode);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            latestVerification.FailedAttempts++;
            if (latestVerification.FailedAttempts >= 5)
            {
                latestVerification.IsUsed = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Error.Validation("OTP.InvalidCode", "Invalid verification code.");
        }
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            latestVerification.IsUsed = true;
            latestVerification.LastModifiedUtc = DateTime.UtcNow;
            var appUser = new ApplicationUser
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                Email = null,
                PhoneNumberConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(appUser);

            if (!createResult.Succeeded)
            {
                var errorDescription = string.Join(" | ", createResult.Errors.Select(e => e.Description));

                return Error.Validation("User.RegistrationFailed", errorDescription);
            }

            var domainUser = new User(appUser.Id)
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IsActive = true
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            var userDto = new AppUserDto(
                appUser.Id,
                appUser.PhoneNumber!,
                roles.ToList()
            );
            _context.Set<User>().Add(domainUser);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(userDto, cancellationToken);

            if (tokenResult.IsError)
            {
                return tokenResult.Errors;
            }

            return tokenResult.Value;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }  
    }

    public string GenerateSecureOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 1_000_000).ToString();
    }
}
