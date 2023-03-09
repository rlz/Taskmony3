using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public class VerificationTokenRepository : BaseRepository<VerificationToken>, IVerificationTokenRepository
{
    public VerificationTokenRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<VerificationToken?> GetAsync(Guid userId, Guid token)
    {
        return await Context.VerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
    }

    public void DeleteByUserId(Guid userId)
    {
        Context.VerificationTokens.RemoveRange(Context.VerificationTokens.Where(x => x.UserId == userId));
    }
}