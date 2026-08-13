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
    private readonly ITelegramService _telegramService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ITokenProvider tokenProvider,
        AppDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ISmsService smsService,
        ITelegramService telegramService)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _context = context;
        _passwordHasher = passwordHasher;
        _smsService = smsService;
        _telegramService = telegramService;
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

        var verification = await _context.PhoneVerifications
        .FirstOrDefaultAsync(pv => pv.PhoneNumber == phoneNumber, cancellationToken);
        if (verification == null || string.IsNullOrEmpty(verification.TelegramChatId))
        {
            return Error.Validation("Telegram.NotFound", "Please start a conversation with the Telegram bot first.");
        }
        if (verification.CreatedAtUtc.AddMinutes(1) > DateTimeOffset.UtcNow)
        {
            return Error.Validation("OTP.RateLimit", "Please wait before requesting another OTP.");
        }
        var randomOtpCode = GenerateSecureOtp();
        string hashedOtp = _passwordHasher.HashPassword(null!, randomOtpCode);
        verification.CreatedAtUtc = DateTimeOffset.UtcNow;
        verification.VerificationCode = hashedOtp;
        verification.ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5);
        verification.IsUsed = false;
        verification.FailedAttempts = 0;
        string userTelegramChatId = verification.TelegramChatId;
        bool isSent = await _telegramService.SendOtpAsync(userTelegramChatId, randomOtpCode);
        if (!isSent)
        {
            return Error.Failure("Telegram.SendFailed", "Failed to send OTP code via Telegram.");
        }
        await _smsService.SendSmsAsync(phoneNumber, $"Your OTP code is: {randomOtpCode}", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
            return Error.Validation("OTP.MaxAttemptsExceeded", "Too many failed attempts. Please request a new code.");
        }
        var verificationResult = _passwordHasher.VerifyHashedPassword(null!, latestVerification.VerificationCode, dto.VerificationCode);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            latestVerification.FailedAttempts++;
            if (latestVerification.FailedAttempts >= 5)
            {
                return Error.Validation("OTP.MaxAttemptsExceeded", "Too many failed attempts. Please request a new code.");
            }

            return Error.Validation("OTP.InvalidCode", "Invalid verification code.");
        }
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            latestVerification.IsUsed = true;
            latestVerification.FailedAttempts = 0;
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
                CreatedAtUtc = DateTime.UtcNow,
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
