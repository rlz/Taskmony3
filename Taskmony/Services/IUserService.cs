using Taskmony.DTOs;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public interface IUserService
{
    Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request);

    Task AddAsync(UserRegisterRequest request);

    /// <summary>
    /// Returns users filtered by the given parameters
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
}