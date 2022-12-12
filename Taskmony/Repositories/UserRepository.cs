using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.DTOs;
using Taskmony.Models;

namespace Taskmony.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskmonyDbContext _context;

    public UserRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<string?> AddUserAsync(User user)
    {
        if (await _context.Users.AnyAsync(x => x.Login == user.Login || x.Email == user.Email))
        {
            return "User with this login or email already exists";
        }

        await _context.Users.AddAsync(user);

        if (!await SaveChangesAsync())
        {
            return "Failed to add user";
        }

        return null;
    }

    public async Task<User?> GetUserAsync(UserAuthRequest request)
    {
        return await _context.Users.FirstOrDefaultAsync(x =>
            x.Login == request.Login && x.Password == request.Password);
    }

    public IQueryable<User> GetUsers(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit)
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