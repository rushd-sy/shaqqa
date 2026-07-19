using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;
using RealEstate.Domain.Identity;
using RealEstate.Domain.Users;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Identity;

public class TokenProvider(IConfiguration configuration , AppDbContext context ,UserManager<User> userManager ) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly AppDbContext _context = context;

    private readonly UserManager<User> _userManager =  userManager;

    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
    {
        var tokenResult = await CreateAsync(user, ct);

        if(tokenResult.IsError)
       { 
         return tokenResult.Errors;
       }

        return  tokenResult.Value;
    }
    
    public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSetting:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = _configuration["JwtSetting:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["JwtSetting:Audience"],
            ValidateLifetime = false, // Ignore token expiration
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
      
    try
    {
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return RefreshTokenErrors.TokenInvalid;
        }

        return principal;
    }
    catch (SecurityTokenException)
    {
        return RefreshTokenErrors.TokenInvalid;
    }
    }


        public async Task<Result<TokenResponse>> CreateAsync(AppUserDto user, CancellationToken ct = default )
    {
        var jwtSettings = _configuration.GetSection("JwtSetting");

        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var key = jwtSettings["SecretKey"]!;
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

        var claim = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub , user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email , user.Email)
          
        };

        foreach (var role in user.Roles)
        {
            claim.Add(new (ClaimTypes.Role,  role));
        }

        var descriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(claim),
          Expires = expires,
          Issuer = issuer,
          Audience = audience,
          SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256Signature)
        }; 

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);
     

        var rawRefreshToken = GenerateRefreshToken();

        var refreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            HashToken(rawRefreshToken),   
            user.UserId,
            DateTime.UtcNow.AddDays(1));

        if (refreshTokenResult.IsError)
        { return refreshTokenResult.Errors; }

        var refreshToken = refreshTokenResult.Value;

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = rawRefreshToken,   
            ExpiresOnUtc = expires
        };
    }  
  public async Task<Result<TokenResponse>> RefreshTokenAsync(string rawRefreshToken, CancellationToken ct = default)
{
    var incomingHash = HashToken(rawRefreshToken);

    var existingToken = await _context.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, ct);

    if (existingToken is null || !existingToken.IsActive)
    {
        return RefreshTokenErrors.TokenRequired;
    }

    var applicationUser = await _userManager.FindByIdAsync(existingToken.UserId.ToString());

    if (applicationUser is null)
    {
        return RefreshTokenErrors.TokenRequired;
    }

    var email = await _userManager.GetEmailAsync(applicationUser);
    var roles = await _userManager.GetRolesAsync(applicationUser);

    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

    existingToken.IsRevoked = true;
    existingToken.RevokeAtUtc = DateTimeOffset.UtcNow;

    var newRawRefreshToken = GenerateRefreshToken();
    var jwtSettings = _configuration.GetSection("JwtSetting");
    var expiresIn = int.Parse(jwtSettings["TokenExpirationInMinutes"]!);
    var expires = DateTime.UtcNow.AddMinutes(expiresIn);

    var newRefreshTokenResult = RefreshToken.Create(
        Guid.NewGuid(),
        HashToken(newRawRefreshToken),
        existingToken.UserId,
        DateTime.UtcNow.AddDays(1));

    if (newRefreshTokenResult.IsError)
    {
        return newRefreshTokenResult.Errors;
    }

    _context.RefreshTokens.Add(newRefreshTokenResult.Value);

    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);

    var claim = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, existingToken.UserId.ToString()),
        new(JwtRegisteredClaimNames.Email, email!)
    };

    foreach (var role in roles)
    {
        claim.Add(new Claim(ClaimTypes.Role, role));
    }

    var descriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claim),
        Expires = expires,
        Issuer = jwtSettings["Issuer"]!,
        Audience = jwtSettings["Audience"]!,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.CreateToken(descriptor);

    return new TokenResponse
    {
        AccessToken = tokenHandler.WriteToken(securityToken),
        RefreshToken = newRawRefreshToken,
        ExpiresOnUtc = expires
    };
}
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}