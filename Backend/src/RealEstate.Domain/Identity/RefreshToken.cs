using RealEstate.Domain.Common;
using RealEstate.Domain.Common.Results;
using RealEstate.Domain.Users;

namespace RealEstate.Domain.Identity;

public class RefreshToken : AuditableEntity
{
    public string TokenHash  { get; private set; }
    public Guid UserId { get; }

    public User User { get; set; } = null! ;
    
    public DateTimeOffset ExpiresOnUtc { get; set; } 
    public DateTimeOffset RevokeAtUtc { get; set; } 

    public bool IsRevoked {get; set;}    
    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken(Guid id, string tokenHash, Guid userId, DateTimeOffset expiresOnUtc)
        : base(id)
    {
        TokenHash  = tokenHash;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }
        public static Result<RefreshToken> Create(Guid id, string tokenHash, Guid userId, DateTimeOffset expiresOnUtc)
    {
        if (id == Guid.Empty)
        {
            return RefreshTokenErrors.IdRequired;
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (userId == Guid.Empty)
        {
            return RefreshTokenErrors.UserIdRequired;
        }

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return new RefreshToken(id, tokenHash, userId, expiresOnUtc);
    }
}