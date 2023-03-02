namespace Taskmony.Models;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }

    public string JwtId { get; set; } = default!;

    public string Token { get; set; } = default!;

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public User User { get; set; } = default!;
}