using HotChocolate.AspNetCore.Authorization;
using Taskmony.Services;

namespace Taskmony.GraphQL.Users;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class UserMutations
{
    [Authorize]
    public async Task<Guid?> UserSetNotificationReadTime([Service] IUserService userService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, string notificationReadTime)
    {
        var notificationReadTimeUtc = timeConverter.StringToDateTimeUtc(notificationReadTime);

        if (await userService.SetNotificationReadTimeAsync(currentUserId, notificationReadTimeUtc, currentUserId))
        {
            return currentUserId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetLogin([Service] IUserService userService, [GlobalState] Guid currentUserId, string login)
    {
        if (await userService.SetLoginAsync(currentUserId, login, currentUserId))
        {
            return currentUserId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetDisplayName([Service] IUserService userService, [GlobalState] Guid currentUserId, string displayName)
    {
        if (await userService.SetDisplayNameAsync(currentUserId, displayName, currentUserId))
        {
            return currentUserId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetEmail([Service] IUserService userService, [GlobalState] Guid currentUserId, string email)
    {
        if (await userService.SetEmailAsync(currentUserId, email, currentUserId))
        {
            return currentUserId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> UserSetPassword([Service] IUserService userService, [GlobalState] Guid currentUserId, string password)
    {
        if (await userService.SetPasswordAsync(currentUserId, password, currentUserId))
        {
            return currentUserId;
        }

        return null;
    }
}