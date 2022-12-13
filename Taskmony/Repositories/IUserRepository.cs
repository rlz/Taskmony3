using Taskmony.Models;

namespace Taskmony.Repositories;

public interface IUserRepository
{
    Task<string?> AddUserAsync(User user);

    Task<User?> GetUserAsync(string login);

    Task<bool> SaveChangesAsync();

    IQueryable<User> GetUsers(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit);
}