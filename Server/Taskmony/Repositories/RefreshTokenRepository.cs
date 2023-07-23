using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Users;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public sealed class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<RefreshToken?> GetAsync(string token)
    {
        return await Context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await Context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
    }
}