using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL.DataLoaders;

public class IdeaByIdDataLoader : BatchDataLoader<Guid, Idea>
{
    private readonly IServiceProvider _serviceProvider;

    public IdeaByIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Idea>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var taskService = scope.ServiceProvider.GetRequiredService<IIdeaService>();

        var tasks = await taskService.GetIdeaByIdsAsync(ids.ToArray());

        return tasks.ToDictionary(t => t.Id);
    }
}