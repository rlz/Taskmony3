using Taskmony.Models;

namespace Taskmony.Services;

public interface IIdeaService
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId,
        int? offset, int? limit, Guid currentUserId);
}