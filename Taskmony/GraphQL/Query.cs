using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL;

public class Query
{
    [Authorize]
    public IQueryable<User> GetUsers([Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        var currentUserId =
            Guid.Parse(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var users = userService.GetUsers(id, email, login, offset, limit, currentUserId);

        return users.Select(x => new User
        {
            Id = x.Id,
            Email = x.Id == currentUserId ? x.Email : null,
            Login = x.Login,
            DisplayName = x.DisplayName
        });
    }
}