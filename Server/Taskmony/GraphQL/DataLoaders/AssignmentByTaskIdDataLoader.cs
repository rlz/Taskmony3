using Taskmony.Models;
using Taskmony.Repositories.Abstract;

namespace Taskmony.GraphQL.DataLoaders;

public class AssignmentByTaskIdDataLoader : BatchDataLoader<Guid, Assignment>
{
    private readonly IServiceProvider _serviceProvider;

    public AssignmentByTaskIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Assignment>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var assignmentRepository = scope.ServiceProvider.GetRequiredService<IAssignmentRepository>();

        var assignments = await assignmentRepository.GetByTaskIdsAsync(ids);

        return assignments.ToDictionary(a => a.TaskId);
    }
}