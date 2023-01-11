using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class DirectionService : IDirectionService
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IUserService _userService;

    public DirectionService(IDirectionRepository directionRepository, IUserService userService)
    {
        _directionRepository = directionRepository;
        _userService = userService;
    }

    public async Task<Direction?> GetDirectionByIdAsync(Guid id)
    {
        return await _directionRepository.GetDirectionByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetDirectionMembersAsync(Guid id, Guid currentUserId)
    {
        var memberIds = await _directionRepository.GetMemberIdsAsync(id);
        return await _userService.GetUsersAsync(memberIds.ToArray(), null, null, null, null, currentUserId);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit,
        Guid currentUserId)
    {
        return await _directionRepository.GetDirectionsAsync(id, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId)
    {
        var userDirections = await _directionRepository.GetDirectionsAsync(null, null, null, userId);
        return userDirections.Select(d => d.Id);
    }
}