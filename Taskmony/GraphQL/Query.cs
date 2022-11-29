using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.GraphQL;

public class Query
{
    [Authorize]
    public IQueryable<User> GetUsers([Service] IUserRepository repository, [Service] IHttpContextAccessor httpContextAccessor,
        [GraphQLType(typeof(ListType<NonNullType<IdType>>))] Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        if (id is null && email is null && login is null)
        {
            var claimsIdentity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var currentUserId = Guid.Parse(claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            return repository.GetUsers(new[] { currentUserId }, null, null, null, null);
        }

        return repository.GetUsers(id, email, login, offset, limit);
    }
}