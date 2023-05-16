using Taskmony.Models.Directions;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.DataLoaders;

public class DirectionByIdDataLoader : BatchDataLoader<Guid, Direction>
{
    private readonly IServiceProvider _serviceProvider;

    public DirectionByIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Direction>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var directionService = scope.ServiceProvider.GetRequiredService<IDirectionService>();

        var directions = await directionService.GetDirectionsByIdsAsync(ids.ToArray());

        return directions.ToDictionary(d => d.Id);
    }
}