using Microsoft.EntityFrameworkCore;
using Taskmony.Data;

namespace Taskmony.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskmonyDbContext _context;

    public UserRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task AddAsync(Models.User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> AnyWithEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> AnyWithLoginAsync(string login)
    {
        return await _context.Users.AnyAsync(x => x.Login == login);
    }

    public async Task<Models.User?> GetByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
    }

    public IQueryable<Models.User> Get(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
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

        if (offset is not null)
        {
            query = query.Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query.Take(limit.Value);
        }

        return query;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}