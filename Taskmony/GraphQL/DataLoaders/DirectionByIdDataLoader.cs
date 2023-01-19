using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL.DataLoaders;

public class DirectionByIdDataLoader : BatchDataLoader<Guid, Direction>
{
    private readonly IDirectionService _directionService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public DirectionByIdDataLoader(IDirectionService directionService, IBatchScheduler batchScheduler,
        DataLoaderOptions options, IUserIdentifierProvider userIdentifierProvider) : base(batchScheduler, options)
    {
        _directionService = directionService;
        _userIdentifierProvider = userIdentifierProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Direction>> LoadBatchAsync(IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var directions = await _directionService.GetDirectionsAsync(keys.ToArray(), null, null, _userIdentifierProvider.UserId);

        return directions.ToDictionary(d => d.Id);
    }
}