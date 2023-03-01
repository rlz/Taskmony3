using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<bool> AnyUserWithEmailAsync(Email email)
    {
        return await Context.Users.AnyAsync(x => x.Email!.Value == email.Value);
    }

    public async Task<bool> AnyUserWithLoginAsync(Login login)
    {
        return await Context.Users.AnyAsync(x => x.Login!.Value == login.Value);
    }

    public async Task<User?> GetByLoginAsync(Login login)
    {
        return await Context.Users.FirstOrDefaultAsync(x => x.Login!.Value == login.Value);
    }

    public async Task<IEnumerable<User>> GetAsync(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
    {
        var query = Context.Users.AsQueryable();

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
}