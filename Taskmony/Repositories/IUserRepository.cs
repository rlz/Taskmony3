namespace Taskmony.Repositories;

public interface IUserRepository
{
    Task AddAsync(Models.User user);

    Task<bool> AnyWithLoginAsync(string login);

    Task<bool> AnyWithEmailAsync(string email);

    Task<Models.User?> GetByLoginAsync(string login);

    Task<bool> SaveChangesAsync();

    IQueryable<Models.User> Get(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit);
}