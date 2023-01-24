using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL.DataLoaders;

public class UserByIdDataLoader : BatchDataLoader<Guid, User>
{
    private readonly IServiceProvider _serviceProvider;

    public UserByIdDataLoader(IBatchScheduler batchScheduler, IServiceProvider serviceProvider,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, User>> LoadBatchAsync(IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userIdentifierProvider = scope.ServiceProvider.GetRequiredService<IUserIdentifierProvider>();

        var users = await userService.GetUsersAsync(ids.ToArray(), null, null, null, null,
            userIdentifierProvider.UserId);

        return users.ToDictionary(u => u.Id);
    }
}