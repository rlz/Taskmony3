using Taskmony.Models;
using Taskmony.ValueObjects;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IUserRepository
{
    Task AddUserAsync(User user);

    Task<bool> AnyUserWithLoginAsync(Login login);

    Task<bool> AnyUserWithEmailAsync(Email email);

    Task<User?> GetUserByLoginAsync(Login login);

    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Gets users filtered by the given parameters
    /// </summary>
    /// <param name="id">an array of the user ids to filter by</param>
    /// <param name="email">an array of the user emails to filter by</param>
    /// <param name="login">an array of the user logins to filter by</param>
    /// <param name="offset">offset of the users sorted by creation date and id</param>
    /// <param name="limit">max number of the users sorted by creation date and id to return</param>
    /// <returns>collection of users</returns>
    Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit);

    Task<bool> SaveChangesAsync();
}