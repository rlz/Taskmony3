using Taskmony.DTOs;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public interface IUserService
{
    Task<UserAuthResponse> AuthenticateUserAsync(UserAuthRequest request);

    Task AddUserAsync(UserRegisterRequest request);

    /// <summary>
    /// Gets users filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the user ids to filter by</param>
    /// <param name="email">an array of the user emails to filter by</param>
    /// <param name="login">an array of the user logins to filter by</param>
    /// <param name="offset">offset of the users sorted by creation date and id</param>
    /// <param name="limit">max number of the users sorted by creation date and id to return</param>
    /// <param name="currentUserId">current user id</param>
    /// <returns>users with fields visible to current user</returns>
    Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login, int? offset,
        int? limit, Guid currentUserId);

    Task<User> GetUserOrThrowAsync(Guid id);

    Task<bool> SetNotificationReadTime(Guid id, DateTime notificationReadTime, Guid currentUserId);

    Task<bool> SetLogin(Guid id, string login, Guid currentUserId);

    Task<bool> SetDisplayName(Guid id, string displayName, Guid currentUserId);

    Task<bool> SetEmail(Guid id, string email, Guid currentUserId);

    Task<bool> SetPassword(Guid id, string password, Guid currentUserId);
}