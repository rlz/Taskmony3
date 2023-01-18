using Taskmony.Models;
using Taskmony.Models.Enums;

namespace Taskmony.Services;

public interface IIdeaService
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId,
        int? offset, int? limit, Guid currentUserId);

    Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId);

    Task<Idea> AddIdea(Idea idea);

    Task<bool> SetIdeaDescription(Guid id, string description, Guid currentUserId);

    Task<bool> SetIdeaDetails(Guid id, string? details, Guid currentUserId);

    Task<bool> SetIdeaDirection(Guid id, Guid? directionId, Guid currentUserId);

    Task<bool> SetIdeaDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId);

    Task<bool> SetIdeaGeneration(Guid id, Generation generation, Guid currentUserId);

    Task<bool> SetIdeaReviewedAt(Guid id, DateTime? reviewedAtUtc, Guid currentUserId);
}