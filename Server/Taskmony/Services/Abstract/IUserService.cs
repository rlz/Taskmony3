using Taskmony.DTOs;
using Taskmony.Models.Users;

namespace Taskmony.Services.Abstract;

public interface IUserService
{
    Task<bool> AddUserAsync(UserRegisterRequest request);

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

    Task<bool> SetNotificationReadTimeAsync(Guid id, DateTime notificationReadTime, Guid currentUserId);

    Task<bool> SetLoginAsync(Guid id, string login, Guid currentUserId);

    Task<bool> SetDisplayNameAsync(Guid id, string displayName, Guid currentUserId);

    Task<bool> SetEmailAsync(Guid id, string email, Guid currentUserId);

    Task<bool> SetPasswordAsync(Guid id, string oldPassword, string newPassword, Guid currentUserId);
}