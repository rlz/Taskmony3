using System.Security.Claims;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class UserIdentifierProvider : IUserIdentifierProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ArgumentException("User identifier claim is required", nameof(httpContextAccessor)));
}