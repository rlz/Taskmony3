using HotChocolate.AspNetCore.Authorization;
using Taskmony.Services;

namespace Taskmony.GraphQL.Users;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class UserMutations
{
    [Authorize]
    public async Task<Guid?> UserSetNotificationReadTime([Service] IUserService userService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, string notificationReadTime, string readAt)
    {
        var notificationReadTimeUtc = timeConverter.StringToDateTimeUtc(notificationReadTime);

        if (await userService.SetNotificationReadTime(userId, notificationReadTimeUtc, userId))
        {
            return userId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetLogin([Service] IUserService userService, [GlobalState] Guid userId, string login)
    {
        if (await userService.SetLogin(userId, login, userId))
        {
            return userId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetDisplayName([Service] IUserService userService, [GlobalState] Guid userId, string displayName)
    {
        if (await userService.SetDisplayName(userId, displayName, userId))
        {
            return userId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetEmail([Service] IUserService userService, [GlobalState] Guid userId, string email)
    {
        if (await userService.SetEmail(userId, email, userId))
        {
            return userId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetPassword([Service] IUserService userService, [GlobalState] Guid userId, string password)
    {
        if (await userService.SetPassword(userId, password, userId))
        {
            return userId;
        }

        return null;
    }
}