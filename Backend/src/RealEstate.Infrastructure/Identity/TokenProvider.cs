using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;
using RealEstate.Domain.Identity;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Identity;

public class TokenProvider(IConfiguration configuration , AppDbContext context ) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly AppDbContext _context = context;
    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
    {
        var tokenResult = await CreateAsync(user, ct);

        if(tokenResult.IsError)
       { 
         return tokenResult.Errors;
       }

        return  tokenResult.Value;
    }
    
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
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
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;
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
     
        var oldRefreshToken = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.UserId)
                .ExecuteDeleteAsync(ct);

        var refreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            GenerateRefreshToken(),
            user.UserId,
            DateTime.UtcNow.AddDays(1));

        if(refreshTokenResult.IsError)
        { return refreshTokenResult.Errors; }

        var refreshToken = refreshTokenResult.Value;
        
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = refreshToken.Token,
            ExpiresOnUtc = expires
        };
    }  
         private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}