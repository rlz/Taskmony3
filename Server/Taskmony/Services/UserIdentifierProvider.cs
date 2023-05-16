using System.Security.Claims;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class UserIdentifierProvider : IUserIdentifierProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? throw new ArgumentException("User identifier claim is required",
                                         nameof(_httpContextAccessor)));
}