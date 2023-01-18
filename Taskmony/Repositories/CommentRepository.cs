using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models.Comments;

namespace Taskmony.Repositories;

public class CommentRepository : ICommentRepository, IAsyncDisposable
{
    private readonly TaskmonyDbContext _context;

    public CommentRepository(IDbContextFactory<TaskmonyDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, int? offset, int? limit)
    {
        var query = _context.TaskComments.AsQueryable();

        query = query.Where(c => c.TaskId == taskId);

        return await GetCommentsAsync(query, offset, limit);
    }

    public async Task<IEnumerable<Comment>> GetIdeaCommentsAsync(Guid ideaId, int? offset, int? limit)
    {
        var query = _context.IdeaComments.AsQueryable();

        query = query.Where(i => i.IdeaId == ideaId);

        return await GetCommentsAsync(query, offset, limit);
    }

    private async Task<IEnumerable<Comment>> GetCommentsAsync(IQueryable<Comment> query, int? offset, int? limit)
    {
        if (offset is not null)
        {
            query = query
                .OrderBy(c => c.CreatedAt)
                .ThenBy(c => c.Id)
                .Skip(offset.Value);
        }

        if (limit is not null)
        {
            query = query
                .OrderBy(c => c.CreatedAt)
                .ThenBy(c => c.Id)
                .Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddComment(TaskComment comment)
    {
        await _context.TaskComments.AddAsync(comment);
    }

    public async Task AddComment(IdeaComment comment)
    {
        await _context.IdeaComments.AddAsync(comment);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}