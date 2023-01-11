using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IDirectionService _directionService;

    public IdeaService(IIdeaRepository ideaRepository, IDirectionService directionService)
    {
        _ideaRepository = ideaRepository;
        _directionService = directionService;
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId, int? offset,
        int? limit, Guid currentUserId)
    {
        //If directionId is [null] return ideas created by the current user with direction id = null
        if (directionId?.Length == 1 && directionId.Contains(null))
        {
            return await _ideaRepository.GetIdeasAsync(id, directionId, offset, limit, currentUserId);
        }

        var userDirectionIds = await _directionService.GetUserDirectionIds(currentUserId);
        var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

        //If directionId is null return all ideas visible to the current user.
        //That includes ideas from all the directions where user is a member
        //(user is a member of his own directions)

        directionId = directionId == null
            ? authorizedDirectionIds.ToArray()
            : directionId.Intersect(authorizedDirectionIds).ToArray();

        return await _ideaRepository.GetIdeasAsync(id, directionId, offset, limit, currentUserId);
    }
}