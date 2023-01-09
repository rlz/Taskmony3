using Taskmony.Models;

namespace Taskmony.Services;

public interface IDirectionService
{
    Task<Direction?> GetDirectionAsync(Guid directionId);
}