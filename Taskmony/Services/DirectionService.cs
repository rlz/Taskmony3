using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class DirectionService : IDirectionService
{
    private readonly IDirectionRepository _directionRepository;

    public DirectionService(IDirectionRepository directionRepository)
    {
        _directionRepository = directionRepository;
    }

    public async Task<Direction?> GetDirectionByIdAsync(Guid id)
    {
        return await _directionRepository.GetDirectionByIdAsync(id);
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId)
    {
        var userDirections = await _directionRepository.GetUserDirectionsAsync(userId);
        return userDirections.Select(d => d.Id);
    }
}