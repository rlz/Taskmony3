namespace Taskmony.Models;

public class VerificationToken : Entity
{
    public Guid Token { get; set; } = default!;

    public Guid UserId { get; set; }

    public User User { get; set; } = default!;
}