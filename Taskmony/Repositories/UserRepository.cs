using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;
using Taskmony.ValueObjects;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public sealed class UserRepository : IUserRepository, IDisposable, IAsyncDisposable
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

    public async Task<bool> AnyUserWithEmailAsync(Email email)
    {
        return await _context.Users.AnyAsync(x => x.Email!.Value == email.Value);
    }

    public async Task<bool> AnyUserWithLoginAsync(Login login)
    {
        return await _context.Users.AnyAsync(x => x.Login!.Value == login.Value);
    }

    public async Task<User?> GetUserByLoginAsync(Login login)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login!.Value == login.Value);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
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
            query = query.Where(x => email.Contains(x.Email!.Value));
        }

        if (login is not null)
        {
            query = query.Where(x => login.Contains(x.Login!.Value));
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
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}