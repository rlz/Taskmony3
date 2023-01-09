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
    
    public async Task<Direction?> GetDirectionAsync(Guid directionId)
    {
        return await _directionRepository.GetDirectionAsync(directionId);
    }
}