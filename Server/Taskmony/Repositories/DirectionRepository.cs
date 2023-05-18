using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Directions;
using Taskmony.Repositories.Abstract;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories;

public sealed class DirectionRepository : BaseRepository<Direction>, IDirectionRepository
{
    public DirectionRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IEnumerable<Direction>> GetAsync(Guid[]? id, bool deleted, DateTime? lastDeletedAt, int? offset,
        int? limit, Guid userId)
    {
        var query = Context.Directions.AsQueryable();

        query = id is null
            ? query.Where(d => d.Members!.Any(m => m.Id == userId))
            : query.Where(d => id.Contains(d.Id) && d.Members!.Any(m => m.Id == userId));

        query = deleted
            ? query.Where(d => d.DeletedAt != null && (lastDeletedAt == null || d.DeletedAt.Value <= lastDeletedAt))
            : query.Where(d => d.DeletedAt == null);

        query = AddPagination(query, offset, limit);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIdsAsync(Guid userId)
    {
        return await Context.Directions
            .Where(d => d.CreatedById == userId || d.Members!.Any(m => m.Id == userId))
            .Select(d => d.Id)
            .ToListAsync();
    }

    private IQueryable<Direction> AddPagination(IQueryable<Direction> query, int? offset, int? limit)
    {
        if (offset is not null)
        {
            query = query
                .OrderBy(d => d.CreatedAt)
                .ThenBy(d => d.Id)
                .Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query
                .OrderBy(d => d.CreatedAt)
                .ThenBy(d => d.Id)
                .Take(limit.Value);
        }

        return query;
    }

    public async Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit)
    {
        var groupedByDirection = Context.Memberships
            .Where(m => directionIds.Contains(m.DirectionId))
            .GroupBy(m => m.DirectionId);

        if (offset is not null && limit is not null)
        {
            return (await groupedByDirection
                    .Select(g =>
                        g.OrderBy(m => m.CreatedAt).ThenBy(m => m.DirectionId).Skip(offset.Value).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g)
                .ToLookup(m => m.DirectionId, m => m.UserId);
        }

        if (offset is not null)
        {
            return (await groupedByDirection
                    .Select(g => g.OrderBy(m => m.CreatedAt).ThenBy(m => m.DirectionId).Skip(offset.Value))
                    .ToListAsync())
                .SelectMany(g => g)
                .ToLookup(m => m.DirectionId, m => m.UserId);
        }

        if (limit is not null)
        {
            return (await groupedByDirection
                    .Select(g => g.OrderBy(m => m.CreatedAt).ThenBy(m => m.DirectionId).Take(limit.Value))
                    .ToListAsync())
                .SelectMany(g => g)
                .ToLookup(m => m.DirectionId, m => m.UserId);
        }

        var memberships = await Context.Memberships.Where(m => directionIds.Contains(m.DirectionId)).ToListAsync();

        return memberships.ToLookup(m => m.DirectionId, m => m.UserId);
    }

    public async Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId)
    {
        return await Context.Memberships.AnyAsync(m => m.DirectionId == directionId && m.UserId == memberId);
    }

    public async Task<bool> AnyMemberInDirectionAsync(Guid directionId)
    {
        return await Context.Memberships.AnyAsync(m => m.DirectionId == directionId);
    }

    public async Task<bool> AnyMemberOtherThanUserInDirectionAsync(Guid directionId, Guid userId)
    {
        return await Context.Memberships.AnyAsync(m => m.DirectionId == directionId && m.UserId != userId);
    }

    public async Task AddMemberAsync(Membership membership)
    {
        await Context.Memberships.AddAsync(membership);
    }

    public void RemoveMember(Membership membership)
    {
        Context.Memberships.Remove(membership);
    }

    public void HardDeleteSoftDeletedDirectionsWithChildren(DateTime deletedBeforeOrAt)
    {
        // Tasks, ideas and comments are deleted with cascade
        Context.Directions.RemoveRange(Context.Directions.Where(d =>
            d.DeletedAt != null && d.DeletedAt.Value <= deletedBeforeOrAt));
    }
}