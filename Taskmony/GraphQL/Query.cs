using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL;

public class Query
{
    [Authorize]
    public IQueryable<User> GetUsers([Service] IUserService userService,
        [Service] IUserIdentifierProvider userIdentifierProvider,
        Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        var users = userService.Get(id, email, login, offset, limit, userIdentifierProvider.UserId);

        return users.Select(x => new User
        {
            Id = x.Id,
            Email = x.Id == userIdentifierProvider.UserId ? x.Email : null,
            Login = x.Login,
            DisplayName = x.DisplayName
        });
    }
}