using System.Security.Claims;

namespace Taskmony.Services;

public class UserIdentifierProvider : IUserIdentifierProvider
{
    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        UserId = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ArgumentException("User identifier claim is required", nameof(httpContextAccessor)));
    }

    public Guid UserId { get; }
}