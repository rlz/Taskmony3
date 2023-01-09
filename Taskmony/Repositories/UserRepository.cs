using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public class UserRepository : IUserRepository, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public UserRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> AnyUserWithEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> AnyUserWithLoginAsync(string login)
    {
        return await _context.Users.AnyAsync(x => x.Login == login);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        var query = _context.Users.AsQueryable();

        if (id is not null)
        {
            query = query.Where(x => id.Contains(x.Id));
        }

        if (email is not null)
        {
            query = query.Where(x => email.Contains(x.Email));
        }

        if (login is not null)
        {
            query = query.Where(x => login.Contains(x.Login));
        }

        query = AddPagination(query, offset, limit);

        return await query.ToListAsync();
    }

    private IQueryable<User> AddPagination(IQueryable<User> query, int? offset, int? limit)
    {
        if (offset is not null)
        {
            query = query
                .OrderBy(u => u.CreatedAt)
                .ThenBy(u => u.Id)
                .Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query
                .OrderBy(u => u.CreatedAt)
                .ThenBy(u => u.Id)
                .Take(limit.Value);
        }

        return query;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}