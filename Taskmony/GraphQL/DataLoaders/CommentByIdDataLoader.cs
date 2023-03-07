using Taskmony.Models.Comments;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.DataLoaders;

public class CommentByIdDataLoader : BatchDataLoader<Guid, Comment>
{
    private readonly IServiceProvider _serviceProvider;

    public CommentByIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Comment>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var commentService = scope.ServiceProvider.GetRequiredService<ICommentService>();

        var comments = await commentService.GetCommentsByIdsAsync(ids.ToArray());

        return comments.ToDictionary(c => c.Id);
    }
}