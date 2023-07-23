using Taskmony.Errors;
using Taskmony.Exceptions;

namespace Taskmony.Models.Users;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }

    public string Token { get; private set; } = default!;

    public bool IsUsed { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public User User { get; private set; } = default!;

    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string token, DateTime? createdAt = null, DateTime? expiresAt = null)
    {
        UserId = userId;
        Token = token;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(30);
    }

    public void Use()
    {
        if (IsUsed)
        {
            throw new DomainException(TokenErrors.RefreshTokenAlreadyUsed);
        }

        if (IsRevoked)
        {
            throw new DomainException(TokenErrors.RefreshTokenRevoked);
        }

        if (ExpiresAt < DateTime.UtcNow)
        {
            throw new DomainException(TokenErrors.RefreshTokenExpired);
        }

        IsUsed = true;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}