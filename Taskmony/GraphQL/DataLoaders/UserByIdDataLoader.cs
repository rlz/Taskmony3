using Taskmony.Models;
using Taskmony.Services;

namespace Taskmony.GraphQL.DataLoaders;

public class UserByIdDataLoader : BatchDataLoader<Guid, User>
{
    private readonly IUserService _userService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UserByIdDataLoader(IUserService userService, IBatchScheduler batchScheduler,
        DataLoaderOptions options, IUserIdentifierProvider userIdentifierProvider) : base(batchScheduler, options)
    {
        _userService = userService;
        _userIdentifierProvider = userIdentifierProvider;
    }

    protected override async Task<IReadOnlyDictionary<Guid, User>> LoadBatchAsync(IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(keys.ToArray(), null, null, null, null, _userIdentifierProvider.UserId);

        return users.ToDictionary(u => u.Id);
    }
}