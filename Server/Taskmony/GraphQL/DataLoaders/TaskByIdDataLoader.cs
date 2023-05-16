using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.GraphQL.DataLoaders;

public class TaskByIdDataLoader : BatchDataLoader<Guid, Task>
{
    private readonly IServiceProvider _serviceProvider;

    public TaskByIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Task>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

        var tasks = await taskService.GetTasksByIdsAsync(ids);

        return tasks.ToDictionary(t => t.Id);
    }
}