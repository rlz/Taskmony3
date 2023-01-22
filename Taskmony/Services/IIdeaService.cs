using Taskmony.Models;
using Taskmony.Models.Enums;

namespace Taskmony.Services;

public interface IIdeaService
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId,
        int? offset, int? limit, Guid currentUserId);

    Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId);

    Task<Idea> AddIdeaAsync(Idea idea);

    Task<bool> SetIdeaDescriptionAsync(Guid id, string description, Guid currentUserId);

    Task<bool> SetIdeaDetailsAsync(Guid id, string? details, Guid currentUserId);

    Task<bool> SetIdeaDirectionAsync(Guid id, Guid? directionId, Guid currentUserId);

    Task<bool> SetIdeaDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId);

    Task<bool> SetIdeaGenerationAsync(Guid id, Generation generation, Guid currentUserId);

    Task<bool> SetIdeaReviewedAtAsync(Guid id, DateTime? reviewedAtUtc, Guid currentUserId);
}